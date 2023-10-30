using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using DataAccess.Repository;
using BusinessObject.Identity;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAPI : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderRepository repository;
        public OrderAPI()
        {
            repository = new OrderRepository();
        }

        // GET: api/OrderAPI
        [HttpGet]
        public  ActionResult<IEnumerable<Order>> GetOrders()
        {
            return repository.GetOrders();
        }

        // GET: api/OrderAPI/5
        [HttpGet("{id}")]
        public ActionResult<Order> GetOrder(int id)
        {
            return repository.GetOrderByID(id);
        }

        // PUT: api/OrderAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutOrder(int id, Order order)
        {
            var o = repository.GetOrderByID(id);
            if(o == null) {
                return NotFound();
            }
            repository.UpdateOrder(o);
            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // POST: api/OrderAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Order> PostOrder(Order order)
        {
            repository.InsertOrder(order); 

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        [HttpGet("orders-by-date")]
        public IActionResult GetOrdersByDate(DateTime from, DateTime to)
        {
            DateTime startDate = from;
            DateTime endDate = to;

            // Query your data store (e.g., database) to retrieve orders within the date range
            var orders = repository.GetOrdersByDateRange(startDate, endDate);

            return Ok(orders);
        }

        // DELETE: api/OrderAPI/5
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(int id)
        {
            var o = repository.GetOrderByID(id);
            if(o == null)
            {
                return NotFound();
            }
            repository.DeleteOrder(id);
            return CreatedAtAction("GetOrder", new {id = o.OrderId}, o);
        }
    }
}
