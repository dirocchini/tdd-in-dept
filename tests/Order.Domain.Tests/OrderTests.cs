using Core.DomainObjects;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using Xunit;

namespace Order.Domain.Tests
{
    public class OrderTests
    {
        [Fact(DisplayName = "Add Item In New Order")]
        [Trait("Category", "Sales - Order")]
        public void AddItem_NewOrder_ShouldUpdateValue()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var item = new Item(Guid.NewGuid(), "product x", 2, 100.00);

            // Act
            order.AddItem(item);

            // Assert
            Assert.Equal(200, order.TotalValue);
        }

        [Fact(DisplayName = "Add Item Already In Order")]
        [Trait("Category", "Sales - Order")]
        public void AddItem_OrderWithItem_ShouldAddItemQuantity()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(itemId, "product x", 2, 100.00);
            order.AddItem(item);
            var item2 = new Item(itemId, "product x", 1, 100.00);

            // Act
            order.AddItem(item2);

            // Assert
            Assert.Equal(300, order.TotalValue);
            Assert.Single(order.Items);
            Assert.Equal(3, order.Items.FirstOrDefault(p => p.Id == itemId)?.Quantity);
        }

        [Fact(DisplayName = "Add Item With Quantity More Than Allowed")]
        [Trait("Category", "Sales - Order")]
        public void AddOrderItem_ItemWithMoreAllowedQuantity_ShouldReturnException()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(itemId, "product x", Order.MAX_ITEM_QUANTITY_PER_ITEM + 1, 100.00);

            // Act & Assert
            Assert.Throws<DomainException>(() => order.AddItem(item));
        }

        [Fact(DisplayName = "Add Item Already In Order With More Quantity Than Allowed")]
        [Trait("Category", "Sales - Order")]
        public void AddItem_OrderWithItemMoreThanAllowedQuantity_ShouldAddItemQuantity()
        {
            // Arrange
            var order = Order.OrderFactory.NewDraftOrder(Guid.NewGuid());
            var itemId = Guid.NewGuid();
            var item = new Item(itemId, "product x", 2, 100.00);
            order.AddItem(item);
            var item2 = new Item(itemId, "product x", Order.MAX_ITEM_QUANTITY_PER_ITEM, 100.00);

            // Act & Assert
            Assert.Throws<DomainException>(() => order.AddItem(item2));
        }
    }

    public class Item
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Quantity { get; private set; }
        public double Value { get; }

        public Item(Guid id, string name, int quantity, double value)
        {
            if (quantity < Order.MIN_ITEM_QUANTITY_PER_ITEM) throw new DomainException($"Item min quantity allowed: {Order.MIN_ITEM_QUANTITY_PER_ITEM}. Quantity received: {quantity}");

            Id = id;
            Name = name;
            Quantity = quantity;
            Value = value;
        }

        public void IncreaseQuantity(int quantity)
        {
            Quantity += quantity;
        }

        public double CalculateValue()
        {
            return Quantity * Value;
        }
    }

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

    public enum OrderStatus
    {
        Draft = 0,
        Started = 1,
        Paid = 4,
        Delivered = 5,
        Cancelled = 6
    }

}