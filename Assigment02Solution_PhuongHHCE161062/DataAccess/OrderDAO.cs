using BusinessObject;
using BusinessObject.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class OrderDAO
    {
        private static OrderDAO instance = null;
        private static readonly object instanceLock = new object();
        public static OrderDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new OrderDAO();
                    }
                    return instance;
                }
            }
        }

        public List<Order> GetOrderList()
        {
            var orders = new List<Order>();
            try
            {
                using var context = new ApplicationDbContext();
                orders = context.Orders.Include(m => m.Member).ToList();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            return orders;
        }

        public List<Order> GetOrdersByDateRange(DateTime startDate, DateTime endDate)
        {
            var orders = new List<Order>();
            try{
                using var context = new ApplicationDbContext();
                orders = context.Orders.Include(m => m.Member).Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate).ToList();
                 
            }catch(Exception e) {
                throw new Exception(e.Message);
            }
            return orders;
        }

        public Order GetOrderByID(int orderId)
        {
            Order order = null;
            try
            {
                using var context = new ApplicationDbContext();
                order = context.Orders.Include(m => m.Member).SingleOrDefault(c => c.OrderId == orderId);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return order;
        }

        public List<Order> GetOrderByMemberId(string memberId)
        {
            var orders = new List<Order>();
            try
            {
                using var context = new ApplicationDbContext();
                orders = context.Orders.Where(m => m.MemberId == memberId).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return orders;
        }

        public void AddNew(Order order)
        {
            try
            {
                Order _order = GetOrderByID(order.OrderId);
                if (_order == null)
                {
                    using var context = new ApplicationDbContext();
                    order.Member = null;
                    context.Orders.Add(order);
                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("The order is already exist.");
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public void Update(Order order)
        {
            try
            {
                Order _order = GetOrderByID(order.OrderId);
                if (_order != null)
                {
                    using var context = new ApplicationDbContext();
                    context.Entry<Order>(order).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
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

        public void Remove(int orderId)
        {
            try
            {
                Order order = GetOrderByID(orderId);
                if (order != null)
                {
                    using var context = new ApplicationDbContext();
                    context.Orders.Remove(order);
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
