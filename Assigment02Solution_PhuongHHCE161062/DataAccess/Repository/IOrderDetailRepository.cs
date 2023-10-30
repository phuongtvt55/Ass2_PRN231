using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IOrderDetailRepository
    {
        List<OrderDetail> GetOrderDetails();
        OrderDetail GetOrderDetailByID(int orderId);
        void InsertOrderDetail(OrderDetail orderDetail);
        void DeleteOrderDetail(int orderId);
        void UpdateOrderDetail(OrderDetail orderDetail);
    }
}
