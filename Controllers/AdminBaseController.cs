using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BookStoreApp.Controllers
{
    public class AdminBaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.Session.GetString("UserRole");
            if (userRole != "Admin")
            {
                context.Result = RedirectToAction("Login", "Account");
            }
            base.OnActionExecuting(context);
        }
    }
}
