using BookStoreApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BookStoreApp.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(); // Enables JSON serialization using Newtonsoft.Json

// Configure DbContext with connection string
builder.Services.AddDbContext<BookStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookStoreDatabase")));

// Configure Stripe settings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

// Add distributed memory cache
builder.Services.AddDistributedMemoryCache();

// Configure session middleware
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Protect cookies from client-side scripts
    options.Cookie.IsEssential = true; // Mark cookie as essential
});

// Register IHttpContextAccessor for accessing HttpContext in services
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Set the Stripe API key globally
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Use custom error page
    app.UseHsts(); // Enforce HTTP Strict Transport Security
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles(); // Serve static files

app.UseRouting();

app.UseSession(); // Enable session middleware
app.UseAuthorization(); // Enable authorization middleware

// Configure endpoint routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();
