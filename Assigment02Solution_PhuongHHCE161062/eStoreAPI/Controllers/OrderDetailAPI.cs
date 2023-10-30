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
    public class OrderDetailAPI : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly OrderDetailRepository repository;
        public OrderDetailAPI()
        {
            repository = new OrderDetailRepository();
        }

        // GET: api/OrderDetailAPI
        [HttpGet]
        public ActionResult<IEnumerable<OrderDetail>> GetOrderDetails()
        {
            return repository.GetOrderDetails();
        }

        // GET: api/OrderDetailAPI/5
        [HttpGet("{id}")]
        public ActionResult<OrderDetail> GetOrderDetail(int id)
        {
            return repository.GetOrderDetailByID(id);
        }

        // PUT: api/OrderDetailAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutOrderDetail(int id, OrderDetail orderDetail)
        {
            var o = repository.GetOrderDetailByID(id);
            if(o == null)
            {
                return NotFound();
            }
            repository.UpdateOrderDetail(o);
            return Ok(o);
        }

        // POST: api/OrderDetailAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<OrderDetail> PostOrderDetail(OrderDetail orderDetail)
        {
            repository.InsertOrderDetail(orderDetail);
            return CreatedAtAction("GetOrderDetail", new { id = orderDetail.ProductId }, orderDetail);
        }

        // DELETE: api/OrderDetailAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var o = repository.GetOrderDetailByID(id);
            if (o == null)
            {
                return NotFound();
            }
            repository.DeleteOrderDetail(id);
            return Ok(o);   
        }
    }
}
