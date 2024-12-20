using Microsoft.AspNetCore.Mvc;
using BookStoreApp.Data;
using BookStoreApp.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.Controllers
{
    public class AdminController : AdminBaseController
    {
        private readonly BookStoreContext _context;

        public AdminController(BookStoreContext context)
        {
            _context = context;
        }



        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddBook(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction("AdminDashboard");
            }

            return View(book);
        }


        // GET: Admin/ManageBooks
        public IActionResult ManageBooks()
        {
            var books = _context.Books.ToList();
            return View(books);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewBook(Book model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var newBook = new Book
                    {
                        Title = model.Title,
                        Author = model.Author,
                        Price = model.Price,
                        StockQuantity = model.StockQuantity,
                        Genre = model.Genre,
                        Description = model.Description,
                        CoverImageUrl = model.CoverImageUrl // Save the cover image URL
                    };

                    _context.Books.Add(newBook);
                    _context.SaveChanges();


                    return RedirectToAction("ManageBooks"); // Ensure the Index action/view exists
                }
                catch (Exception ex)
                {
                    // Log the error (not shown)
                    ModelState.AddModelError("", "An error occurred while saving the book. Please try again.");
                }
            }

            // Return the model to the form if validation fails or an exception occurs
            return View(model);
        }


        // GET: Admin/EditBook/{id}
        public IActionResult EditBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Admin/EditBook/{id}
        [HttpPost]
        public IActionResult EditBook(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                _context.SaveChanges();
                return RedirectToAction("ManageBooks");
            }
            return View(book);
        }

        // GET: Admin/DeleteBook/{id}
        public ActionResult DeleteBook(int id)
        {
            // Find the book by its ID
            var book = _context.Books.SingleOrDefault(b => b.Id == id);

            if (book == null)
            {
                // If the book is not found, return a 404 error page or redirect
                return HttpNotFound();
            }

            // Remove the book from the database
            _context.Books.Remove(book);
            _context.SaveChanges();

            // Redirect back to the book list page or wherever you need
            return RedirectToAction("ManageBooks", "Admin"); // Change to your book list view
        }

        private ActionResult HttpNotFound()
        {
            throw new NotImplementedException();
        }


        // Manage Orders Actions
        public IActionResult ManageOrders()
        {
            var orders = _context.Orders.ToList();
            return View(orders);
        }

        public IActionResult UpdateOrderStatus(int id, string status)
        {
            // Find the order by its ID and include the related OrderItems
            var order = _context.Orders
                                .Include(o => o.OrderItems)  // Eagerly load OrderItems
                                .ThenInclude(oi => oi.Book)  // If Book is a navigation property within OrderItems
                                .FirstOrDefault(o => o.Id == id); // Use FirstOrDefault instead of Find for better control

            if (order != null)
            {
                // Update the order status
                order.Status = status;

                // If the status is "Complete" or "Cancelled"
                if (status == "Complete" || status == "Cancelled")
                {
                    // Assuming each order has a list of order items, with a relationship to books
                    foreach (var orderItem in order.OrderItems) // OrderItems is now eagerly loaded
                    {
                        var book = orderItem.Book; // Assuming Book is a navigation property in OrderItem
                        if (book != null)
                        {
                            // Reduce the book's quantity by the quantity ordered
                            book.StockQuantity -= orderItem.Quantity;

                            // Ensure the quantity doesn't go below 0
                            if (book.StockQuantity < 0)
                            {
                                book.StockQuantity = 0; // Optionally, you could throw an exception or handle it differently
                            }
                        }
                    }
                }

                // Save the changes to both the order and the books
                _context.SaveChanges();

                // Pass the alert message to the view
                TempData["AlertMessage"] = $"Order status has been updated to {status}.";
            }

            // Redirect back to the ManageOrders page
            return RedirectToAction("ManageOrders");
        }


        // POST: UpdateStockQuantity
        [HttpPost]
        public IActionResult UpdateStockQuantity(int id, int stockQuantity)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book != null)
            {
                book.StockQuantity = stockQuantity;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Stock quantity updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Book not found.";
            }
            return RedirectToAction("ManageBooks");
        }


        // POST: UpdateBook
        [HttpPost]
        public IActionResult UpdateBook(Book updatedBook)
        {
            if (!ModelState.IsValid)
            {
                var existingBook = _context.Books.FirstOrDefault(b => b.Id == updatedBook.Id);
                if (existingBook != null)
                {
                    existingBook.Title = updatedBook.Title;
                    existingBook.Author = updatedBook.Author;
                    existingBook.Price = updatedBook.Price;
                    existingBook.Description = updatedBook.Description;
                    existingBook.Genre = updatedBook.Genre;
                    existingBook.StockQuantity = updatedBook.StockQuantity;

                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Book details updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Book not found.";
                }
            }
            return RedirectToAction("ManageBooks");
        }

    }
}
