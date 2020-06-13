using Core.DomainObjects;
using Microsoft.VisualBasic;
using Sales.Domain;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using Xunit;

namespace Domain.Tests
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

}