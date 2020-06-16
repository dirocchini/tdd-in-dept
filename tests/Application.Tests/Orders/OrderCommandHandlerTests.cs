using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using Moq.AutoMock;
using Sales.Application.Commands;
using Sales.Domain;
using Xunit;

namespace Application.Tests.Orders
{
    public class OrderCommandHandlerTests
    {
        [Fact(DisplayName = "Add New Order Item to New Order Successfully")]
        [Trait("Category", "Sales - Order Command Handler")]
        public async void AddOrderItem_NewOrder_ShouldAddItem()
        {
            // Arrange
            var command = new AddOrderItemCommand(new Guid(),new Guid(),"item x", 2, 100 );
            var mocker = new AutoMocker();
            var orderHandler = mocker.CreateInstance<OrderCommandHandler>();

            mocker.GetMock<IOrderRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await orderHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            mocker.GetMock<IOrderRepository>().Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
            mocker.GetMock<IOrderRepository>().Verify(r=> r.UnitOfWork.Commit(), Times.Once);
            //mocker.GetMock<IMediator>().Verify(r => r.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }
    }
}
