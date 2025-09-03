using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web_CuaHangCafe.Helpers;
using Web_CuaHangCafe.Models;

public class ShoppingCartSummaryViewComponent : ViewComponent
{
    public Task<IViewComponentResult> InvokeAsync()
    {
        var cartItems = HttpContext.Session.Get<List<CartItem>>("Cart");
        var cartItemCount = (cartItems != null) ? cartItems.Count : 0;

        return Task.FromResult<IViewComponentResult>(View(cartItemCount));
    }
}
