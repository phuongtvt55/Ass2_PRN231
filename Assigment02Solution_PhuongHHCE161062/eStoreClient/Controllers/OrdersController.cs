using BusinessObject;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using BusinessObject.Identity;
using System.Linq;
using eStoreClient.Session;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace eStoreClient.Controllers
{
    [Authorize]
    public class OrdersController: Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpClient client;
        private string api;

        public OrdersController()
        {
            this._context = new ApplicationDbContext();
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            api = "https://localhost:44307/api/OrderAPI";
        }        
        public IActionResult AddToCart(int id)
        {
            var checko = User.IsInRole("Admin");
            /*var memId = HttpContext.Session.GetInt32("MemberId");
            if (memId == null)
            {
                TempData["errorMessage"] = "You must to login first to add to cart";
                //return RedirectToAction("Login", "Members");
            }*/
            var quantity = HttpContext.Session.GetInt32("quantity");
            if (quantity == null)
            {
                quantity = 1;
            }
            var product = _context.Products.FirstOrDefault(s => s.ProductId == id);
            var price = product.UnitPrice;
            List<OrderDetail> cart = HttpContext.Session.GetObject<List<OrderDetail>>("cart");
            if (cart != null)
            {
                foreach (var item in cart)
                {
                    if (item.ProductId == id && (product.UnitslnStock - item.Quantity == 0))
                    {
                        TempData["errorMessage"] = "Product is out of stock";
                        return RedirectToAction("Index", "Products");
                    }
                }
                var check = cart.FirstOrDefault(s => s.ProductId == id);

                if (check == null)
                {
                    OrderDetail sp = new OrderDetail()
                    {
                        ProductId = id,
                        UnitPrice = (decimal)(price * quantity),
                        Quantity = (int)quantity,
                        Discount = 0,
                        Product = product
                    };
                    cart.Add(sp);
                }
                else
                {
                    check.Quantity++;
                    check.UnitPrice = price * check.Quantity;
                }
                HttpContext.Session.SetObject("cart", cart);
            }
            else
            {
                cart = new List<OrderDetail>();
                OrderDetail sp = new OrderDetail()
                {
                    ProductId = id,
                    UnitPrice = (decimal)(price * quantity),
                    Quantity = (int)quantity,
                    Discount = 0,
                    Product = product
                };
                cart.Add(sp);
                HttpContext.Session.SetObject("cart", cart);
            }
            var list = HttpContext.Session.GetObject<List<OrderDetail>>("cart");
            var sum = 0;
            foreach (var item in list)
            {
                sum += item.Quantity;
            }
            HttpContext.Session.SetInt32("bag", sum);
            HttpContext.Session.Remove("quantity");
            return RedirectToAction("Index", "Products");
        }

        public ActionResult ViewOrder(int id)
        {
            var orderDetail = new List<OrderDetail>();
            var memId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            try
            {
                using var context = new ApplicationDbContext();
                orderDetail = context.OrderDetails.Include(b => b.Product).Include(o => o.Order).Where(c => c.OrderId == id).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return View(orderDetail);
        }

        public ActionResult ViewCart()
        {
            //var memId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            /*if (memId == null)
            {
                TempData["errorMessage"] = "You must to login first to see cart";
                return RedirectToAction("Login", "Members");
            }*/
            List<OrderDetail> list = HttpContext.Session.GetObject<List<OrderDetail>>("cart");
            if (list != null)
            {
                List<OrderDetail> list1 = new List<OrderDetail>();
                var sum = 0;
                foreach (var d in list)
                {
                    sum += Convert.ToInt32(d.UnitPrice);
                    list1.Add(d);
                }
                ViewBag.TotalPrice = sum;
                return View(list1);
            }
            else
            {
                return View(list);
            }

        }

        [HttpPost]
        public ActionResult Update_Quantity_Cart(IFormCollection form)
        {
            List<OrderDetail> list = HttpContext.Session.GetObject<List<OrderDetail>>("cart");

            var status = form["status"];
            var proid = int.Parse(form["proId"]);
            var product = _context.Products.FirstOrDefault(s => s.ProductId == proid);
            var quantity = int.Parse(form["quantity"]);
            var price = product.UnitPrice;

            if (list != null)
            {
                int count = 0;
                foreach (var item in list)
                {
                    if (item.ProductId == proid)
                    {
                        count++;
                    }
                }
                if (count == 0)
                {
                    HttpContext.Session.SetInt32("quantity", quantity);
                    AddToCart(proid);
                    return RedirectToAction("Index", "Products");
                }
            }
            else
            {
                HttpContext.Session.SetInt32("quantity", quantity);
                AddToCart(proid);
                return RedirectToAction("Index", "Products");
            }

            var check = list.FirstOrDefault(s => s.ProductId == proid);
            if (status.Equals("detail"))
            {
                if (check == null)
                {
                    check.Quantity = quantity;
                }
                else
                {
                    check.Quantity += quantity;
                }
            }
            else
            {
                check.Quantity = quantity;
            }
            check.UnitPrice = check.Quantity * price;
            HttpContext.Session.SetObject("cart", list);
            var list1 = HttpContext.Session.GetObject<List<OrderDetail>>("cart");
            var sum = 0;
            foreach (var item in list1)
            {
                sum += item.Quantity;
            }
            HttpContext.Session.SetInt32("bag", sum);

            return RedirectToAction("ViewCart");
        }

        public ActionResult Remove(int id)
        {
            List<OrderDetail> list = HttpContext.Session.GetObject<List<OrderDetail>>("cart");
            var check = list.FirstOrDefault(s => s.ProductId == id);
            list.Remove(check);
            HttpContext.Session.SetObject("cart", list);
            var list1 = HttpContext.Session.GetObject<List<OrderDetail>>("cart");
            var sum = 0;
            foreach (var item in list1)
            {
                sum += item.Quantity;
            }
            HttpContext.Session.SetInt32("bag", sum);
            return RedirectToAction("ViewCart");
        }

        public IActionResult Checkout(DateTime requiredDate)
        {
            var memId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Order order = new Order();
            if (requiredDate < DateTime.Now.AddDays(3))
            {
                TempData["errorMessage"] = "Select date at least 4 days";
                return RedirectToAction("ViewCart");
            }
            order.RequiredDate = requiredDate;
            Random rnd = new Random();
            order.ShippedDate = requiredDate.AddDays(rnd.Next(4));
            order.Freight = 0;
            order.MemberId = memId;
            List<OrderDetail> list = HttpContext.Session.GetObject<List<OrderDetail>>("cart");
            if (list == null)
            {
                TempData["errorMessage"] = "Please order some product";
                return RedirectToAction("Index", "Products");
            }
            _context.Orders.Add(order);
            _context.SaveChanges();

            foreach (var item in list)
            {
                OrderDetail orderDetail = new OrderDetail();
                orderDetail.OrderId = order.OrderId;
                orderDetail.ProductId = item.ProductId;
                orderDetail.Quantity = item.Quantity;
                orderDetail.UnitPrice = item.UnitPrice * item.Quantity;
                orderDetail.Discount = item.Discount;
                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges();
                using (var context = new ApplicationDbContext())
                {
                    Product product = null;
                    product = context.Products.SingleOrDefault(s => s.ProductId == item.ProductId);
                    product.UnitslnStock -= item.Quantity;
                    context.Products.Update(product);
                    context.SaveChanges();
                }
            }
            list.Clear();
            HttpContext.Session.SetObject("cart", list);
            HttpContext.Session.Remove("bag");
            TempData["orderSuccess"] = "Order success";
            return RedirectToAction("Index", "Products");
        }

        // GET: Orders
        public async Task<IActionResult> Index(DateTime from, DateTime to)
        {
            HttpResponseMessage respone;
            var memId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string formattedFrom = from.ToString("MM/dd/yyyy");
            string formattedTo = to.ToString("MM/dd/yyyy");
            if (from != DateTime.MinValue && to != DateTime.MinValue)
            {
                respone = await client.GetAsync(api + "/orders-by-date?from=" + formattedFrom + "&to=" + formattedTo);
                //respone = await client.GetAsync(api + "/orders-by-date?from09/27/2023&to09/29/2023");
            }
            else
            {
                respone = await client.GetAsync(api);
            }
            string data = await respone.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            List<Order> orders = System.Text.Json.JsonSerializer.Deserialize<List<Order>>(data, options);
            if (User.IsInRole("User"))
            {
                orders = orders.Where(m => m.MemberId.Equals(memId)).ToList();
            }
            return View(orders);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpResponseMessage response = await client.GetAsync(api + "/" + id);
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                Order order = System.Text.Json.JsonSerializer.Deserialize<Order>(data, options);
                return View(order);
            }
            return NotFound();
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            //ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "City");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "City", order.MemberId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
           // ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "City", order.MemberId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,MemberId,OrderDate,RequiredDate,ShippedDate,Freight")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            //ViewData["MemberId"] = new SelectList(_context.Ca, "MemberId", "City", order.MemberId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Member)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
