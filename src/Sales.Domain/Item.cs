using Core.DomainObjects;
using System;

namespace Sales.Domain
{
    public class Item
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Quantity { get; private set; }
        public decimal Value { get; }

        public Item(Guid id, string name, int quantity, decimal value)
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

        public decimal CalculateValue()
        {
            return Quantity * Value;
        }
    }

}