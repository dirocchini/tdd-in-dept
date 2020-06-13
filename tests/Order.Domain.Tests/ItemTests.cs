using Core.DomainObjects;
using Sales.Domain;
using System;
using Xunit;

namespace Domain.Tests
{
    public class ItemTests
    {
        [Fact(DisplayName = "Create Item With Quantity Less Than Allowed")]
        [Trait("Category", "Sales - Order - Item")]
        public void CreateItem_ItemWithLessAllowed_ShouldReturnException()
        {
            // Arrange & Act & Assert
            Assert.Throws<DomainException>(() => new Item(Guid.NewGuid(), "product x", Order.MIN_ITEM_QUANTITY_PER_ITEM - 1, 100.00));
        }
    }
}
