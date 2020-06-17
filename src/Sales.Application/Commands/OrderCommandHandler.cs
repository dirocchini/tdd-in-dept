using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.DomainObjects;
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
            if (!await IsCommandValid(message, cancellationToken)) return false;

            var order = await _orderRepository.GetDraftOrderByClientId(message.ClientId);
            var item = new Item(message.ItemId, message.ItemName, message.ItemQuantity, message.ItemValue);

            if (order == null)
            {
                order = Order.OrderFactory.NewDraftOrder(message.ClientId);
                order.AddItem(item);
                _orderRepository.Add(order);
            }
            else
            {
                var existingItem = order.ItemExists(item);
                order.AddItem(item);

                if (existingItem)
                {
                    _orderRepository.UpdateOrderItem(order.Items.FirstOrDefault(i => i.Id == item.Id));
                }
                else
                {
                    _orderRepository.AddOrderItem(item);
                }
                _orderRepository.UpdateOrder(order);
            }

            order.AddEvent(new OrderItemAddedEvent(order.ClientId, order.Id, message.ItemId, message.ItemName, message.ItemQuantity, message.ItemValue));
            return await _orderRepository.UnitOfWork.Commit();
        }

        private async Task<bool> IsCommandValid(AddOrderItemCommand message, CancellationToken cancellationToken)
        {
            if (message.IsValid()) return true;

            foreach (var error in message.ValidationResult.Errors)
            {
                await _mediator.Publish(new DomainNotification(message.MessageType, error.ErrorMessage), cancellationToken);
            }

            return false;

        }
    }
}