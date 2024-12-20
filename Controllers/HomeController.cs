using Microsoft.AspNetCore.Mvc;
using BookStoreApp.Data;
using System.Linq;

namespace BookStoreApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly BookStoreContext _context;

        public HomeController(BookStoreContext context)
        {
            _context = context;
        }

        // Display the main page with optional search functionality
        public IActionResult Index(string searchQuery)
        {
            // Fetch all books if no search query is provided
            var books = string.IsNullOrEmpty(searchQuery)
                ? _context.Books.ToList()
                : _context.Books
                    .Where(b => b.Title.Contains(searchQuery) ||
                                b.Author.Contains(searchQuery) ||
                                b.Description.Contains(searchQuery))
                    .ToList();

            // Pass the search query back to the view for display in the search bar
            ViewData["SearchQuery"] = searchQuery;
            return View(books);
        }

        // Display the book details page for a specific book
        public IActionResult BookDetails(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null)
            {
                return NotFound(); // Return 404 if the book doesn't exist
            }

            return View(book);
        }
    }
}
