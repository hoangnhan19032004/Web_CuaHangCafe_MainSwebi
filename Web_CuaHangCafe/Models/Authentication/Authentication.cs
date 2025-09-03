using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web_CuaHangCafe.Models.Authentication
{
    public class AuthenticationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var user = session.GetString("Admin");

            if (string.IsNullOrEmpty(user))
            {
                context.Result = new RedirectToActionResult("Login", "Accounts", null);
            }
        }
    }
}
