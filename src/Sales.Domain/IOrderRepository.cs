using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Core.Data;

namespace Sales.Domain
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Add(Order order);
        void UpdateOrder(Order order);
        void AddOrderItem(Item isAny);
        void UpdateOrderItem(Item isAny);
        Task<Order> GetDraftOrderByClientId(Guid clientId);
    }
}
