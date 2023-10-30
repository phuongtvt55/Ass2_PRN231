using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BusinessObject;
using DataAccess.Repository;

namespace eStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPI : ControllerBase
    {
        
        private readonly IProductRepository repository;
        private readonly ICategoryRepository categoryRepository;
        public ProductAPI()
        {           
            repository = new ProductRepository();
            categoryRepository = new CategoryRepository();
        }

        // GET: api/Products
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            return repository.GetProducts();
        }

        [HttpGet("GetCategory")]
        public IActionResult GetCategory()
        {
            return Ok(categoryRepository.GetCategorys());
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public ActionResult<Product> GetProduct(int id)
        {
            return repository.GetProductById(id);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutProduct(int id, Product product)
        {
            var p = repository.GetProductById(id);
            if(p == null)
            {
                return NotFound();
            }
            product.ProductId = p.ProductId;
            repository.UpdateProduct(product);
            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<Product> PostProduct(Product product)
        {
            repository.InsertProduct(product);

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = repository.GetProductById(id);
            if(product == null)
            {
                return NotFound();
            }
            repository.DeleteProduct(id);
            return CreatedAtAction("GetProduct", new {id = product.ProductId}, product);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Product>> SearchProduct(string name)
        {
            return repository.SearchProductName(name);
        }
    }
}
