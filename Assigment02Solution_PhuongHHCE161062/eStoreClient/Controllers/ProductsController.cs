using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessObject.Identity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eStoreClient.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient client;
        private string api = "";

        public ProductsController()
        {
            this._context = new ApplicationDbContext();
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            api = "https://localhost:44307/api/ProductAPI";
        }

        // GET: Products
        public async Task<IActionResult> Index(string searchString)
        {
            
            var name = searchString;
            HttpResponseMessage response;
            if (name != null)
            {
                response = await client.GetAsync(api + "/search?name=" + name);
            }
            else
            {
                response = await client.GetAsync(api);
            }

            string data = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };

            List<Product> list = JsonSerializer.Deserialize<List<Product>>(data, options);

            return View(list);
        }

        [HttpGet]
        [Route("product/price/{min}/{max}")]
        public IActionResult FilterPrice(decimal min, decimal max)
        {
            var producs = _context.Products.Where(p => p.UnitPrice >= min && p.UnitPrice <= max).ToList();
            var p = _context.Products.ToList();
            return new JsonResult(producs);
        }

        [HttpGet]
        public JsonResult SearchProducts(string customerName)
        {
            var products = _context.Products.Where(p => p.ProductName.Contains(customerName));
            return new JsonResult(products);
        }
    }
}
