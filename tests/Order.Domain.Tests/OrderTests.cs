using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
            var order = new Order();
            var item = new Item(Guid.NewGuid, "product x", 2, 100.00);


            // Act
            order.AddItem(item);

            // Assert
            Assert.Equal(200, order.TotalValue);

        }
    }

    public class Item
    {
        public Func<Guid> Id { get; }
        public string Name { get; }
        public int Quantity { get; }
        public double Value { get; }

        public Item(Func<Guid> id, string name, int quantity, double value)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
            Value = value;
        }
    }

    public class Order
    {
        public Order()
        {
            _items = new Collection<Item>();
        }

        public double TotalValue { get; private set; }
        
        private readonly Collection<Item> _items;
        public IReadOnlyCollection<Item> Items => _items;

        public void AddItem(Item item)
        {
            _items.Add(item);
            TotalValue = Items.Sum(i => i.Value * i.Quantity);
        }
    }
}