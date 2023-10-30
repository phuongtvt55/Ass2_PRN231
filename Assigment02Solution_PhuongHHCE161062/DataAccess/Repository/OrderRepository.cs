using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public Order GetOrderByID(int orderId) => OrderDAO.Instance.GetOrderByID(orderId);
        public List<Order> GetOrders() => OrderDAO.Instance.GetOrderList();
        public void InsertOrder(Order order) => OrderDAO.Instance.AddNew(order);
        public void DeleteOrder(int orderId) => OrderDAO.Instance.Remove(orderId);
        public void UpdateOrder(Order order) => OrderDAO.Instance.Update(order);
        
        public List<Order> GetOrdersSigleMember(string memberId) => OrderDAO.Instance.GetOrderByMemberId(memberId);

        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate) => OrderDAO.Instance.GetOrdersByDateRange(startDate, endDate);        
    }
}
