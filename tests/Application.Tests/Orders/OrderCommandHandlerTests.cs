using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Win32.SafeHandles;
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
            var command = new AddOrderItemCommand(Guid.NewGuid(), Guid.NewGuid(), "item x", 2, 100);
            var mocker = new AutoMocker();
            var orderHandler = mocker.CreateInstance<OrderCommandHandler>();

            mocker.GetMock<IOrderRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await orderHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            mocker.GetMock<IOrderRepository>().Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
            mocker.GetMock<IOrderRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
            //mocker.GetMock<IMediator>().Verify(r => r.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Once);
        }

        [Fact(DisplayName = "Add New Order Item to Draft Order Successfully")]
        [Trait("Category", "Sales - Order Command Handler")]
        public async void AddOrderItem_OrderInDraft_ShouldAddItem()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var order = Order.OrderFactory.NewDraftOrder(clientId);
            var itemAlreadyAdded = new Item(Guid.NewGuid(), "item x", 1, 100);
            order.AddItem(itemAlreadyAdded);

            var command = new AddOrderItemCommand(clientId, Guid.NewGuid(), "item z", 2, 100);

            var mocker = new AutoMocker();
            var orderHandler = mocker.CreateInstance<OrderCommandHandler>();

            mocker.GetMock<IOrderRepository>().Setup(r => r.GetDraftOrderByClientId(clientId))
                .Returns(Task.FromResult(order));
            mocker.GetMock<IOrderRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await orderHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            mocker.GetMock<IOrderRepository>().Verify(r => r.AddOrderItem(It.IsAny<Item>()), Times.Once);
            mocker.GetMock<IOrderRepository>().Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Once);
            mocker.GetMock<IOrderRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Add Existing Order Item to Draft Order Successfully")]
        [Trait("Category", "Sales - Order Command Handler")]
        public async void AddOrderItem_ExistingItemInDraftOrder_ShouldAddItem()
        {
            // Arrange
            var clientId = Guid.NewGuid();
            var itemId = Guid.NewGuid();

            var order = Order.OrderFactory.NewDraftOrder(clientId);
            var existingItem = new Item(itemId, "item x", 1, 100);
            order.AddItem(existingItem);

            var command = new AddOrderItemCommand(clientId, itemId, "item x", 2, 50);

            var mocker = new AutoMocker();
            var orderHandler = mocker.CreateInstance<OrderCommandHandler>();

            mocker.GetMock<IOrderRepository>().Setup(r => r.GetDraftOrderByClientId(clientId))
                .Returns(Task.FromResult(order));
            mocker.GetMock<IOrderRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await orderHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            mocker.GetMock<IOrderRepository>().Verify(r => r.UpdateOrderItem(It.IsAny<Item>()), Times.Once);
            mocker.GetMock<IOrderRepository>().Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Once);
            mocker.GetMock<IOrderRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }


        [Fact(DisplayName = "Add Order Item Invalid Command")]
        [Trait("Category", "Sales - Order Command Handler")]
        public async void AddOrderItem_PassInvalidCommand_ShouldReturnFalseAndThrowException()
        {
            // Arrange
            var command = new AddOrderItemCommand(Guid.Empty, Guid.Empty, string.Empty, 0,0);

            var mocker = new AutoMocker();
            var orderHandler = mocker.CreateInstance<OrderCommandHandler>();

            // Act
            var result = await orderHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            mocker.GetMock<IMediator>().Verify(r => r.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Exactly(5));

        }
    }
}