using System;
using System.Collections.Generic;
using System.Text;
using Core.Data;

namespace Sales.Domain
{
    public interface IOrderRepository : IRepository<Order>
    {
        void Add(Order order);
    }
}
