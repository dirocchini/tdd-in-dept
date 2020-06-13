﻿using Microsoft.VisualBasic;
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
        [Fact(DisplayName = "Add Item New Order")]
        [Trait("Category", "Order")]
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
        [Trait("Category", "Order")]
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
    }

    public class Item
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Quantity { get; private set; }
        public double Value { get; }

        public Item(Guid id, string name, int quantity, double value)
        {
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
            if (_items.Any(p => p.Id == item.Id))
            {
                var existingItem = _items.FirstOrDefault(p => p.Id == item.Id);
                existingItem.IncreaseQuantity(item.Quantity);
                item = existingItem;

                _items.Remove(existingItem);
            }

            _items.Add(item);
            CalculateValue();
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