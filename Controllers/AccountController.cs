using Microsoft.AspNetCore.Mvc;
using BookStoreApp.Data;
using BookStoreApp.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BookStoreApp.Controllers
{
    public class AccountController : BaseController
    {
        private readonly BookStoreContext _context;

        public AccountController(BookStoreContext context)
        {
            _context = context;
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and Password are required.");
                return View();
            }

            // Hash the password for comparison
            var hashedPassword = HashPassword(password);

            // Verify email and password
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            // Set session for the logged-in user
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName); // Optional for personalization
            HttpContext.Session.SetString("UserRole", user.Role); // Add this line


            return RedirectToAction("Index", "Home");
        }

        // GET: Signup
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        // POST: Signup
        [HttpPost]
        public IActionResult Register(string name, string email, string password, string confirmPassword)
        {
            // Log the received form data
            Console.WriteLine($"Form Data: name={name}, email={email}, password={password}, confirmPassword={confirmPassword}");

            // Check if passwords match
            Console.WriteLine($"Password: {password}, ConfirmPassword: {confirmPassword}");
            if (password != confirmPassword)
            {
                Console.WriteLine("Passwords do not match");
                ModelState.AddModelError("Password", "Passwords do not match.");
                return View("Signup");
            }

            // Check for duplicate email
            if (_context.Users.Any(u => u.Email == email))
            {
                Console.WriteLine("Email already exists");
                ModelState.AddModelError("Email", "This email is already registered.");
                return View("Signup");
            }

            // Hash the password
            var hashedPassword = HashPassword(password);

            // Create and save new user
            var newUser = new User
            {
                FullName = name,
                Email = email,
                Password = hashedPassword
            };

            Console.WriteLine("Adding user to the context");
            _context.Users.Add(newUser);

            try
            {
                Console.WriteLine("Saving changes to the database");
                _context.SaveChanges();
                Console.WriteLine("User successfully saved");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving user: {ex.Message}");
            }

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // Helper Method to Hash Passwords
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
