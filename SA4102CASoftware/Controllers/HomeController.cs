using SA4102CASoftware.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace SA4102CASoftware.Controllers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value, Formatting.None,
                        new JsonSerializerSettings()
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        }));
        }

        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static int GetCartQuantity(this ISession session)
        {
            var cartQuantities = session.GetObjectFromJson<List<int>>("cartQuantities");
            return cartQuantities?.Sum() ?? 0;
        }
    }
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext db;

        public HomeController(ILogger<HomeController> logger, MyDbContext db)
        {
            _logger = logger;
            this.db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // get all products and product average ratings 
            var model = GetProducts();

            return View(model);
        }

        // generate view model with all product information and product average ratings
        public List<ProductRatingViewModel> GetProducts()
        {

            var products = db.Products.ToList();
            var model = new List<ProductRatingViewModel>();

            foreach (var product in products)
            {
                var orderDetails = db.OrderDetails.Where(o =>
                 o.ProductId == product.ProductId);
                var averageRating = orderDetails.Any() ? orderDetails.Average(o => o.Rating) : 0;
                var productWithAverageRating = new ProductRatingViewModel
                {
                    Product = product,
                    AverageRating = averageRating / 5 * 100
                };
                model.Add(productWithAverageRating);
            }
            return model;
        }



        [HttpGet]
        public IActionResult LiveTagSearch(string q)
        {
            var model = GetProducts();

            // if there is no search value, return view with all products shown
            if (string.IsNullOrEmpty(q))
            {
                
                return PartialView("_SearchResults", model);
            }
            // else, show only products with names or descriptions that match with search value
            else
            {

                List<ProductRatingViewModel> res = model
                .Where(p => p.Product.ProductName.ToLower().Contains(q.ToLower()) || p.Product.ProductDescription.ToLower().Contains(q.ToLower())).ToList();

                return PartialView("_SearchResults", res);
            }
        }



        [HttpPost]
        public IActionResult ShoppingCart(string productId, int quantity, string quantityInput)
        {
            // find product with matching productId
            var product = db.Products.FirstOrDefault(p => p.ProductId == productId);

            if (product == null)
            {
                return NotFound();
            }
            
            // use sessions to store selected products and their corresponding quantities 
            var cart = HttpContext.Session.GetObjectFromJson<List<Products>>("cart");
            var cartQuantities = HttpContext.Session.GetObjectFromJson<List<int>>("cartQuantities");

            if (cart == null)
            {
                cart = new List<Products>();
                cartQuantities = new List<int>();
            }

            // check if product exists in shopping cart
            int index = cart.FindIndex(p => p.ProductId == productId);


            if (index >= 0)
            {
                // If the product is already in the cart, update the quantity
                if (!string.IsNullOrEmpty(quantityInput))
                {
                    cartQuantities[index] = int.Parse(quantityInput);
                    HttpContext.Session.SetObjectAsJson("cart", cart);
                    HttpContext.Session.SetObjectAsJson("cartQuantities", cartQuantities);
                    return Redirect("Cart");
                }
                else
                {
                    cartQuantities[index]++;
                }
            }
            else
            {
                // If the product is not in the cart, add it with the specified quantity
                cart.Add(product);
                cartQuantities.Add(1);
            }

            // update session with updated cart and quantities 
            HttpContext.Session.SetObjectAsJson("cart", cart);
            HttpContext.Session.SetObjectAsJson("cartQuantities", cartQuantities);

            return RedirectToAction("Index");
        }



        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Products>>("cart");
            var cartQuantities = HttpContext.Session.GetObjectFromJson<List<int>>("cartQuantities");

            // Check the cart, qty and count to display the message if null
            if (cart == null || cartQuantities == null || cart.Count == 0 || cartQuantities.Count == 0)
            {
                return View();
            }

            // Create a list of tuples to combine each product with its quantity
            var cartItems = new List<(Products product, int quantity)>();
            for (int i = 0; i < cart.Count; i++)
            {
                cartItems.Add((cart[i], cartQuantities[i]));
            }

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(string productId)
        {
            try
            {
                // Get the cart and cartQuantities from the session
                var cart = HttpContext.Session.GetObjectFromJson<List<Products>>("cart") ?? new List<Products>();
                var cartQuantities = HttpContext.Session.GetObjectFromJson<List<int>>("cartQuantities") ?? new List<int>();

                // Find the index of the product to be removed in the cart
                int index = cart.FindIndex(p => p.ProductId.Equals(productId));

                // If the product exists in the cart, remove it from the cart and cartQuantities lists
                if (index >= 0)
                {
                    cart.RemoveAt(index);
                    cartQuantities.RemoveAt(index);
                }

                // Update the cart and cartQuantities in the session
                HttpContext.Session.SetObjectAsJson("cart", cart);
                HttpContext.Session.SetObjectAsJson("cartQuantities", cartQuantities);



                // Return a JSON response indicating the success of the operation
                return Json(new { success = true });
            }
            catch
            {
                // Return a JSON response indicating the failure of the operation
                return Json(new { success = false });
            }
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<Products>>("cart");
            var cartQuantities = HttpContext.Session.GetObjectFromJson<List<int>>("cartQuantities");


            // Check if cart is empty
            if (cart == null || cartQuantities == null || cart.Count != cartQuantities.Count)
            {
                return BadRequest();
            }

            var email = HttpContext.Session.GetString("Email");

            // Prompt user to login if not yet done so
            if (email == null)
            {
                return RedirectToAction("Login", "Login");
            }

            // Get the email of the current user
            Customers customer = db.Customers.FirstOrDefault(x => x.Email == email);

            // Create a new order
            Orders order = new Orders
            {
                Customer = customer,
                OrderDate = DateTime.Now
            };

            // Add the order to the database
            db.Orders.Add(order);

            for (int i = 0; i < cart.Count; i++)
            {
                var product = db.Products.Find(cart[i].ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                // Create a new order detail
                OrderDetails orderDetail = new OrderDetails
                {
                    Orders = order,
                    Products = product,
                    Quantity = cartQuantities[i]
                };

                // Add the order detail to the database
                db.OrderDetails.Add(orderDetail);
            }

            db.SaveChanges();


            // Create activation code for new orders
            OrderDetails[] orderDetails = db.OrderDetails.Where(od => od.OrderId == order.OrderId).ToArray();

            for (int i = 0; i < orderDetails.Length; i++)
            {
                for (int j = 0; j < orderDetails[i].Quantity; j++)
                {
                    db.Add(new ActivationCodes
                    {
                        OrderId = orderDetails[i].OrderId,
                        ProductId = orderDetails[i].ProductId,
                    });
                }
            }

            // Save changes to the database
            db.SaveChanges();

            // Clear shopping cart
            HttpContext.Session.Remove("cart");
            HttpContext.Session.Remove("cartQuantities");
            return RedirectToAction("MyPurchases", "Purchases");
        }


        [HttpGet]
        public JsonResult CalculateTotal()
        {
            // Retrieve items from the cart    
            var cart = HttpContext.Session.GetObjectFromJson<List<Products>>("cart");
            var cartQuantities = HttpContext.Session.GetObjectFromJson<List<int>>("cartQuantities");
            double totalAmount = 0;


            // For each item, calculate subtotal and add to total amount
            for (int i = 0; i < cart.Count; i++)
            {
                string productId = cart[i].ProductId;

                Products product = db.Products.FirstOrDefault(x =>
                x.ProductId == productId);

                if (product != null)
                {
                    double productPrice = product.Price;
                    double subTotal = productPrice * cartQuantities[i];
                    totalAmount += subTotal;
                }

            }

            return Json(new { total = totalAmount });

        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Confirmation()
        {

            return View();
        }
    }
}