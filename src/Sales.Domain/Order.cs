using Core.DomainObjects;
using FluentValidation.Results;
using Sales.Domain.VoucherEntities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sales.Domain
{
    public class Order : Entity, IAggregateRoot
    {
        public static int MAX_ITEM_QUANTITY_PER_ITEM => 15;
        public static int MIN_ITEM_QUANTITY_PER_ITEM => 1;


        public Guid ClientId { get; private set; }
        public decimal TotalValue { get; private set; }
        public OrderStatus OrderStatus { get; private set; }
        private readonly Collection<Item> _items;
        public IReadOnlyCollection<Item> Items => _items;

        public Voucher Voucher { get; private set; }
        public bool VoucherUsed { get; private set; }
        public decimal DiscountValue { get; private set; }

        protected Order()
        {
            _items = new Collection<Item>();
        }





        public void AddItem(Item item)
        {
            CheckAllowedItemQuantity(item);

            if (ItemExists(item))
            {
                var existingItem = _items.FirstOrDefault(p => p.Id == item.Id);

                existingItem.IncreaseQuantity(item.Quantity);
                item = existingItem;

                _items.Remove(existingItem);
            }

            _items.Add(item);
            CalculateValue();
        }

        private void CheckAllowedItemQuantity(Item item)
        {
            var quantity = item.Quantity;
            if (ItemExists(item))
            {
                var existingItem = _items.FirstOrDefault(p => p.Id == item.Id);
                quantity += existingItem.Quantity;
            }

            if (quantity > MAX_ITEM_QUANTITY_PER_ITEM) throw new DomainException($"Item max quantity allowed: {MAX_ITEM_QUANTITY_PER_ITEM}. Quantity received: {item.Quantity}");
        }

        public bool ItemExists(Item item)
        {
            return _items.Any(p => p.Id == item.Id);
        }

        private void CalculateValue()
        {
            TotalValue = _items.Sum(i => i.CalculateValue());
            CalculateTotalDiscountValue();
        }

        public void SetOrderStatusDraft()
        {
            OrderStatus = OrderStatus.Draft;
        }

        public class OrderFactory
        {
            public static Order NewDraftOrder(Guid clientId)
            {
                var order = new Order
                {
                    ClientId = clientId
                };

                order.SetOrderStatusDraft();
                return order;
            }
        }

        public void UpdateItem(Item item)
        {
            CheckItemInOrder(item);
            CheckAllowedItemQuantity(item);

            var existingItem = _items.FirstOrDefault(i => i.Id == item.Id);
            _items.Remove(existingItem);
            _items.Add(item);

            CalculateValue();
        }

        private void CheckItemInOrder(Item item)
        {
            if (!ItemExists(item)) throw new DomainException($"Item {item.Id} not in order");
        }

        public void RemoveItem(Item item)
        {
            CheckItemInOrder(item);

            var existingItem = _items.FirstOrDefault(i => i.Id == item.Id);
            _items.Remove(existingItem);

            CalculateValue();

        }

        public ValidationResult ApplyVoucher(Voucher voucher)
        {
            var result = voucher.Validate();
            if (!result.IsValid) return result;

            Voucher = voucher;
            VoucherUsed = true;

            CalculateTotalDiscountValue();

            return result;
        }

        private void CalculateTotalDiscountValue()
        {
            if (!VoucherUsed) return;

            if (Voucher.VoucherType == VoucherType.Value)
            {
                if (Voucher.DiscountValue.HasValue)
                {
                    DiscountValue = Voucher.DiscountValue.Value;
                    TotalValue -= DiscountValue;
                }
            }
            else
            {
                if (Voucher.DiscountPercentage.HasValue)
                {
                    DiscountValue = TotalValue * Voucher.DiscountPercentage.Value / 100;
                    TotalValue -= DiscountValue;
                }
            }

            TotalValue = TotalValue <= 0 ? 0 : TotalValue;
        }
    }
}