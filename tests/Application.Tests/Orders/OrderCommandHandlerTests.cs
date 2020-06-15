using System;
using System.Collections.Generic;
using System.Text;
using Sales.Application.Commands;
using Xunit;

namespace Application.Tests.Orders
{
    public class OrderCommandHandlerTests
    {
        [Fact(DisplayName = "Add New Order Item to New Order Successfully")]
        [Trait("Category", "Sales - Order Command Handler")]
        public void AddOrderItem_NewOrder_ShouldAddItem()
        {
            // Arrange
            var command = new AddOrderItemCommand(new Guid(),new Guid(),"item x", 2, 100 );

            // Act

            // Assert
                
        }
    }
}
