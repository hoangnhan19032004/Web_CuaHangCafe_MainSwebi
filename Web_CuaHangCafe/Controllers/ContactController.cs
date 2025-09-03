using Microsoft.AspNetCore.Mvc;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;

namespace Web_CuaHangCafe.Controllers
{
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(TbPhanHoi phanHoi)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    phanHoi.Id = Guid.NewGuid();
                    phanHoi.NgayPhanHoi = DateTime.Now;

                    _context.TbPhanHois.Add(phanHoi);
                    _context.SaveChanges();

                    TempData["Status"] = "success";
                    TempData["Message"] = "Gửi thành công";

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    TempData["Status"] = "error";
                    TempData["Message"] = "Không gửi được lời nhắn: " + ex.Message;
                }
            }
            else
            {
                // Ghi log lỗi để tiện debug
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"Lỗi {entry.Key}: {error.ErrorMessage}");
                    }
                }

                TempData["Status"] = "error";
                TempData["Message"] = "Vui lòng kiểm tra lại thông tin.";
            }

            return View(phanHoi);
        }
    }
}
