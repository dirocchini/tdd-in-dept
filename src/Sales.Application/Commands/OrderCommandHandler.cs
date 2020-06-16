using System.Data;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sales.Application.Events;
using Sales.Domain;

namespace Sales.Application.Commands
{
    public class OrderCommandHandler : IRequestHandler<AddOrderItemCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMediator _mediator;

        public OrderCommandHandler(IOrderRepository orderRepository, IMediator mediator)
        {
            _orderRepository = orderRepository;
            _mediator = mediator;
        }

        public async Task<bool> Handle(AddOrderItemCommand message, CancellationToken cancellationToken)
        {
            var item = new Item(message.ItemId, message.ItemName, message.ItemQuantity, message.ItemValue);
            var order = Order.OrderFactory.NewDraftOrder(message.ClientId);
            order.AddItem(item);

            _orderRepository.Add(order);

            await _mediator.Publish(new OrderItemAddedEvent(order.ClientId, order.Id, message.ItemId, message.ItemName, message.ItemQuantity, message.ItemValue), cancellationToken);
            return true;
        }

    }
}