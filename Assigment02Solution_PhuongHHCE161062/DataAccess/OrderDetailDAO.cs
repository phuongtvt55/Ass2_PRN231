using BusinessObject;
using BusinessObject.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OrderDetailDAO
    {
        private static OrderDetailDAO instance = null;
        private static readonly object instanceLock = new object();
        public static OrderDetailDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDetailDAO();
                    }
                    return instance;
                }
            }
        }
        //-------------------------------------------------------------
        public List<OrderDetail> GetOrderDetailList()
        {
            var orderDetails = new List<OrderDetail>();
            try
            {
                using var context = new ApplicationDbContext();
                orderDetails = context.OrderDetails.Include(p => p.Product).Include(o => o.Order).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderDetails;
        }
        //-------------------------------------------------------------
        public OrderDetail GetOrderDetailByID(int orderId)
        {
            OrderDetail orderDetail = null;
            try
            {
                using var context = new ApplicationDbContext();
                orderDetail = context.OrderDetails.Include(p => p.Product).Include(o => o.Order).SingleOrDefault(od => od.OrderId == orderId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return orderDetail;
        }
        //-------------------------------------------------------------
        public void AddNew(OrderDetail orderDetail)
        {
            try
            {
                OrderDetail _orderDetail = GetOrderDetailByID(orderDetail.OrderId);
                if (_orderDetail == null)
                {
                    using var context = new ApplicationDbContext();
                    orderDetail.Product = null;
                    orderDetail.Order = null;
                    context.OrderDetails.Add(orderDetail);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The order already existed.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //-------------------------------------------------------------
        public void Update(OrderDetail orderDetail)
        {
            try
            {
                OrderDetail _orderDetail = GetOrderDetailByID(orderDetail.OrderId);
                if (_orderDetail == null)
                {
                    using var context = new ApplicationDbContext();
                    context.Entry<OrderDetail>(orderDetail).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The order does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //-------------------------------------------------------------
        public void Remove(int orderId)
        {
            try
            {
                OrderDetail orderDetail = GetOrderDetailByID(orderId);
                if (orderDetail != null)
                {
                    using var context = new ApplicationDbContext();
                    context.OrderDetails.Remove(orderDetail);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The order does not already exist.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
