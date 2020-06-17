using System;
using System.Collections.Generic;
using System.Text;
using Core.Messages;
using MediatR;

namespace Sales.Application.Events
{
    public class OrderItemAddedEvent : Event
    {
        public Guid ClientId { get; private set; }
        public Guid OrderId { get; private set; }
        public Guid ItemId { get; }
        public string ItemName { get; }
        public int ItemQuantity { get; }
        public decimal ItemValue { get; }


        public OrderItemAddedEvent(Guid clientId, Guid orderId, Guid itemId, string itemName, int itemQuantity, decimal itemValue)
        {
            ClientId = clientId;
            OrderId = orderId;
            ItemId = itemId;
            ItemName = itemName;
            ItemQuantity = itemQuantity;
            ItemValue = itemValue;
        }
    }
}
