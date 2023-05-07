using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SA4102CASoftware.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SA4102CASoftware.Controllers
{
    public class LoginController : Controller
    {
        private readonly MyDbContext db;

        public LoginController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        // login page for the user
        public IActionResult Login()
        {
            if (!bool.Parse(HttpContext.Session.GetString("LoggedIn")))
                return View();
            // If user is already logged in, redirect to home page
            return RedirectToAction("Index", "Home");
        }

        // after user enters email and password, to redirect to relevant page
        [HttpPost]
        public IActionResult ValidateLogin(string email, string password)
        {
            // Check if an entry exists
            Customers? loggedIn = db.Customers.SingleOrDefault(u => u.Email == email && u.Password == password);

            // Compare the password as mysql query return is not case senstive
            bool checkPass = loggedIn != null && string.Equals(password, loggedIn.Password);

            // if correct email and password, to save sessions for user details
            if (loggedIn != null && checkPass)
            {
                HttpContext.Session.SetString("FirstName", (string)loggedIn.FirstName);
                HttpContext.Session.SetString("LastName", (string)loggedIn.LastName);
                HttpContext.Session.SetString("LoggedIn", "true");
                HttpContext.Session.SetString("Email", (string)loggedIn.Email);

                // redirect to the login success view page
                return RedirectToAction("LoginSuccess");
            }

            // if unsuccessful log in, redirect to login page again
            else
            {
                HttpContext.Session.SetString("LoggedIn", "false");
                ViewBag.Error = "Incorrect email or password.";
                return View("Login");
            }
        }

        [HttpGet]
        // display the login success view page
        public ActionResult LoginSuccess()
        {
            return View();
        }


        [HttpGet]
        // logout page for the user
        public IActionResult Logout()
        {
            // Check that that user is logged in, if logged in, redirect to logout
            bool loginStatus = bool.Parse(HttpContext.Session.GetString("LoggedIn"));
            if (loginStatus)
            {
                HttpContext.Session.SetString("LoggedIn", "false");
                HttpContext.Session.Remove("Email");
                // Clear cart when user log out
                HttpContext.Session.Remove("cartQuantities");
                HttpContext.Session.Remove("cart");
                return View();
            }
            // When the user is not logged in, there wont be a logout button
            // This else statement is just to safeguard in case the user access the logout from the url
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}