using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IOrderRepository
    {
        List<Order> GetOrders();
        List<Order> GetOrdersSigleMember(string memberId);
        List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate);
        Order GetOrderByID(int orderId);
        void InsertOrder(Order order);
        void DeleteOrder(int orderId);
        void UpdateOrder(Order order);
    }
}
