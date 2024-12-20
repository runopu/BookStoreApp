using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using BookStoreApp.Models;
using BookStoreApp.Helpers;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.Data;

namespace BookStoreApp.Controllers
{
    public class PaymentController : Controller
    {
        private readonly StripeSettings _stripeSettings;
        private readonly BookStoreContext _context;


        public PaymentController(IOptions<StripeSettings> stripeOptions, BookStoreContext context)
        {
            _stripeSettings = stripeOptions.Value;
            _context = context;
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession()
        {
            // Retrieve the cart from session
            var cart = HttpContext.Session.Get<List<Book>>("Cart") ?? new List<Book>();
            if (!cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var domain = $"{Request.Scheme}://{Request.Host}";

            // Build line items for the session
            var lineItems = cart.Select(book => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(book.Price * 100), // Stripe expects the amount in cents
                    Currency = "cad",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = book.Title,
                        Description = book.Author
                    },
                },
                Quantity = 1, // Adjust as needed
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{domain}/Payment/Success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{domain}/Payment/Cancel",
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Json(new { id = session.Id });
        }

        [HttpGet]
        public IActionResult Success(string session_id)
        {
            var service = new SessionService();
            var session = service.Get(session_id);

            // Get the user's email from the session or session metadata
            var userEmail = HttpContext.Session.GetString("UserEmail");

            // Retrieve the cart from session (if needed)
            var cart = HttpContext.Session.Get<List<Book>>("Cart") ?? new List<Book>();

            // Create a new order
            var order = new Order
            {
                UserEmail = userEmail,
                OrderDate = DateTime.Now,
                Status = "Processing",
                OrderItems = cart.Select(book => new OrderItem
                {
                    BookId = book.Id,
                    Price = book.Price,
                    Quantity = 1
                }).ToList()
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Clear the cart
            HttpContext.Session.Remove("Cart");

            return View(order);
        }


        [HttpGet]
        public IActionResult Cancel()
        {
            return View();
        }
    }
}
