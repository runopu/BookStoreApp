using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewData["IsLoggedIn"] = HttpContext.Session.GetString("UserEmail") != null;
        base.OnActionExecuting(context);
    }
}


