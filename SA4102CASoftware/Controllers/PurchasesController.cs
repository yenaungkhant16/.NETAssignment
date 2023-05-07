using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SA4102CASoftware.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SA4102CASoftware.Controllers
{
    public class PurchasesController : Controller
    {

        private readonly MyDbContext db;

        public PurchasesController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult MyPurchases()
        {
            // Step 1: If the person is not logged in, redirect to the login page
            if (!bool.Parse(HttpContext.Session.GetString("LoggedIn")))
            {
                return RedirectToAction("Login", "Login");
            }

            // Step 2: Get the user who is logged in from database
            string loggedInEmail = HttpContext.Session.GetString("Email");

            // Step 3: Generate the details required for the purchase details
            // This will return a view model that will be passed to the view
            List<MyPurchasesModel> purchasesModel = GetPurchasesTable(loggedInEmail);
            return View(purchasesModel);
        }

        // This method will return a list of purchases 
        private List<MyPurchasesModel> GetPurchasesTable(string loggedInEmail)
        {
            // Joined table from orders, order details, products
            var result = (from order in db.Orders
                          join detail in db.OrderDetails on order.OrderId equals detail.OrderId
                          join product in db.Products on detail.ProductId equals product.ProductId
                          where order.Email == loggedInEmail
                          orderby order.OrderDate descending
                          select new
                          {
                              OrderId = order.OrderId,
                              Email = order.Email,
                              OrderDate = order.OrderDate,
                              ProductId = product.ProductId,
                              ProductName = product.ProductName,
                              ProductPrice = product.Price,
                              ProductDescription = product.ProductDescription,
                              ProductImage = product.ProductImage,
                              Quantity = detail.Quantity,
                              Rating = detail.Rating
                          });

            // Joined the result table with activation code and save it to a list to iterate through
            var resultWithActivationCode = (
                from r in result
                join act in db.ActivationCodes on new { r.OrderId, r.ProductId } equals new { act.OrderId, act.ProductId }
                select new PurchasesTable
                {
                    OrderId = r.OrderId,
                    ProductId = r.ProductId,
                    ProductName = r.ProductName,
                    ProductDescription = r.ProductDescription,
                    ProductImage = r.ProductImage,
                    OrderDate = r.OrderDate,
                    Quantity = r.Quantity,
                    Rating = r.Rating,
                    ActivationCode = act.ActivationCode
                }
            ).ToList();

            // Store the data from the database into the view model
            var purchasesList = new List<MyPurchasesModel>();

            foreach (var item in resultWithActivationCode)
            {
                var purchase = new MyPurchasesModel
                {
                    OrderId = item.OrderId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    ProductDescription = item.ProductDescription,
                    ProductImage = item.ProductImage,
                    OrderDate = item.OrderDate,
                    Quantity = item.Quantity,
                    Rating = item.Rating,
                    ActivationCode = item.ActivationCode
                };

                purchasesList.Add(purchase);
            }

            return purchasesList;
        }

        [HttpPost]
        public IActionResult SubmitReview(double rating, string productId, Guid orderId)
        {
           // Find order with matching orderId and productId
            OrderDetails? order = db.OrderDetails.FirstOrDefault(
                x => x.OrderId == orderId && x.ProductId == productId);


            // Update rating if order is found
            if (order != null)
            {
                order.Rating = rating;
                db.SaveChanges();
            }


            return RedirectToAction("MyPurchases", "Purchases");
        }
    }
}

