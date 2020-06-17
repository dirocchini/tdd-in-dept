using System;
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
        private readonly Guid _clientId;
        private readonly Guid _itemId;
        private readonly Order _order;
        private readonly AutoMocker _mocker;
        private readonly OrderCommandHandler _orderCommandHandler;


        public OrderCommandHandlerTests()
        {
            _mocker = new AutoMocker();
            _orderCommandHandler = _mocker.CreateInstance<OrderCommandHandler>();
            _clientId = Guid.NewGuid();
            _itemId = Guid.NewGuid();
            _order = Order.OrderFactory.NewDraftOrder(_clientId);
        }


        [Fact(DisplayName = "Add New Order Item to New Order Successfully")]
        [Trait("Category", "Sales - Order Command Handler")]
        public async void AddOrderItem_NewOrder_ShouldAddItem()
        {
            // Arrange
            var command = new AddOrderItemCommand(Guid.NewGuid(), Guid.NewGuid(), "item x", 2, 100);

            _mocker.GetMock<IOrderRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _orderCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<IOrderRepository>().Verify(r => r.Add(It.IsAny<Order>()), Times.Once);
            _mocker.GetMock<IOrderRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Add New Order Item to Draft Order Successfully")]
        [Trait("Category", "Sales - Order Command Handler")]
        public async void AddOrderItem_OrderInDraft_ShouldAddItem()
        {
            // Arrange
            var itemAlreadyAdded = new Item(Guid.NewGuid(), "item x", 1, 100);
            _order.AddItem(itemAlreadyAdded);

            var command = new AddOrderItemCommand(_clientId, Guid.NewGuid(), "item z", 2, 100);

            _mocker.GetMock<IOrderRepository>().Setup(r => r.GetDraftOrderByClientId(_clientId)).Returns(Task.FromResult(_order));
            _mocker.GetMock<IOrderRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _orderCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<IOrderRepository>().Verify(r => r.AddOrderItem(It.IsAny<Item>()), Times.Once);
            _mocker.GetMock<IOrderRepository>().Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Once);
            _mocker.GetMock<IOrderRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }

        [Fact(DisplayName = "Add Existing Order Item to Draft Order Successfully")]
        [Trait("Category", "Sales - Order Command Handler")]
        public async void AddOrderItem_ExistingItemInDraftOrder_ShouldAddItem()
        {
            // Arrange
            var existingItem = new Item(_itemId, "item x", 1, 100);
            _order.AddItem(existingItem);

            var command = new AddOrderItemCommand(_itemId, _itemId, "item x", 2, 50);

            _mocker.GetMock<IOrderRepository>().Setup(r => r.GetDraftOrderByClientId(_itemId)).Returns(Task.FromResult(_order));
            _mocker.GetMock<IOrderRepository>().Setup(r => r.UnitOfWork.Commit()).Returns(Task.FromResult(true));

            // Act
            var result = await _orderCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mocker.GetMock<IOrderRepository>().Verify(r => r.UpdateOrderItem(It.IsAny<Item>()), Times.Once);
            _mocker.GetMock<IOrderRepository>().Verify(r => r.UpdateOrder(It.IsAny<Order>()), Times.Once);
            _mocker.GetMock<IOrderRepository>().Verify(r => r.UnitOfWork.Commit(), Times.Once);
        }


        [Fact(DisplayName = "Add Order Item Invalid Command")]
        [Trait("Category", "Sales - Order Command Handler")]
        public async void AddOrderItem_PassInvalidCommand_ShouldReturnFalseAndThrowException()
        {
            // Arrange
            var command = new AddOrderItemCommand(Guid.Empty, Guid.Empty, string.Empty, 0, 0);

            // Act
            var result = await _orderCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mocker.GetMock<IMediator>().Verify(r => r.Publish(It.IsAny<INotification>(), CancellationToken.None), Times.Exactly(5));
        }
    }
}