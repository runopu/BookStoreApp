using Microsoft.AspNetCore.Mvc;
using BookStoreApp.Data;
using BookStoreApp.Models;
using BookStoreApp.Helpers;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Controllers
{
    public class CartController : BaseController
    {
        private const string CartSessionKey = "Cart";
        private readonly BookStoreContext _context;

        public CartController(BookStoreContext context)
        {
            _context = context;
        }

        // Display Cart Page
        public IActionResult Index()
        {
            // Retrieve cart from session or initialize an empty one
            var cart = HttpContext.Session.Get<List<Book>>(CartSessionKey) ?? new List<Book>();
            return View(cart);
        }

        // Add a book to the cart
        public IActionResult AddToCart(int id)
        {
            // Ensure user is logged in by checking session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the book by ID
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            // Retrieve cart or initialize a new one
            var cart = HttpContext.Session.Get<List<Book>>(CartSessionKey) ?? new List<Book>();
            cart.Add(book);

            // Save the updated cart back to session
            HttpContext.Session.Set(CartSessionKey, cart);

            return RedirectToAction("Index");
        }


        // Remove a book from the cart
        public IActionResult RemoveFromCart(int id)
        {
            var cart = HttpContext.Session.Get<List<Book>>(CartSessionKey) ?? new List<Book>();
            var bookToRemove = cart.FirstOrDefault(b => b.Id == id);
            if (bookToRemove != null)
            {
                cart.Remove(bookToRemove);
            }

            HttpContext.Session.Set(CartSessionKey, cart);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.Get<List<Book>>(CartSessionKey) ?? new List<Book>();
            if (cart.Any())
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Create a new order
                var order = new Order
                {
                    UserEmail = userEmail,
                    OrderDate = DateTime.Now,
                    Status = "Processing",
                    OrderItems = new List<OrderItem>()
                };

                foreach (var book in cart)
                {
                    var orderItem = new OrderItem
                    {
                        BookId = book.Id,
                        Price = book.Price,
                        Quantity = 1
                    };
                    order.OrderItems.Add(orderItem);
                }

                _context.Orders.Add(order);
                _context.SaveChanges();

                // Clear the cart
                HttpContext.Session.Remove(CartSessionKey);

                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                Console.WriteLine("Order not found for ID: " + orderId);
                return NotFound();
            }
            foreach (var item in order.OrderItems)
            {
                Console.WriteLine($"Book Title: {item.Book?.Title ?? "Book is null"}, Price: {item.Price}");
            }
            return View(order);
        }


    }
}
