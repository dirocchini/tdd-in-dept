using Core.DomainObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sales.Domain
{
    public class Order
    {
        public static int MAX_ITEM_QUANTITY_PER_ITEM => 15;
        public static int MIN_ITEM_QUANTITY_PER_ITEM => 1;


        protected Order()
        {
            _items = new Collection<Item>();
        }

        public Guid ClientId { get; private set; }
        public double TotalValue { get; private set; }
        public OrderStatus OrderStatus { get; private set; }
        private readonly Collection<Item> _items;
        public IReadOnlyCollection<Item> Items => _items;



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

        private bool ItemExists(Item item)
        {
            return _items.Any(p => p.Id == item.Id);
        }

        private void CalculateValue()
        {
            TotalValue = _items.Sum(i => i.CalculateValue());
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
    }

}