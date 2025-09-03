using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Accounts")]
    public class AccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(TbQuanTriVien model)
        {
            if (string.IsNullOrWhiteSpace(model.TenNguoiDung) || string.IsNullOrWhiteSpace(model.MatKhau))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                return View(model);
            }

            var user = _context.TbQuanTriViens
                .AsNoTracking()
                .FirstOrDefault(u =>
                    u.TenNguoiDung.ToLower() == model.TenNguoiDung.ToLower()
                    && u.MatKhau == model.MatKhau
                );

            if (user != null)
            {
                HttpContext.Session.SetString("Admin", user.TenNguoiDung);
                return RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
            }

            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng!");
            return View(model);
        }

        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Admin");
            return RedirectToAction("Login");
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index(int? page)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Admin")))
                return RedirectToAction("Login", "Accounts");

            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listItem = _context.TbQuanTriViens
                .AsNoTracking()
                .OrderBy(x => x.TenNguoiDung)
                .ToList();

            var pagedListItem = new PagedList<TbQuanTriVien>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Admin")))
                return RedirectToAction("Login", "Accounts");

            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TbQuanTriVien quanTriVien)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Admin")))
                return RedirectToAction("Login", "Accounts");

            // Lưu mật khẩu như người dùng nhập (không mã hoá)
            _context.TbQuanTriViens.Add(quanTriVien);
            _context.SaveChanges();
            TempData["Message"] = "Thêm thành công";

            return RedirectToAction("Index", "Accounts");
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Admin")))
                return RedirectToAction("Login", "Accounts");

            var quanTriVien = _context.TbQuanTriViens.Find(id);
            if (quanTriVien == null)
            {
                TempData["Message"] = "Không tìm thấy quản trị viên.";
                return RedirectToAction("Index");
            }

            return View(quanTriVien);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TbQuanTriVien quanTriVien)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Admin")))
                return RedirectToAction("Login", "Accounts");

            var existingUser = _context.TbQuanTriViens.AsNoTracking().FirstOrDefault(x => x.Id == quanTriVien.Id);
            if (existingUser == null)
            {
                TempData["Message"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }

            _context.Entry(quanTriVien).State = EntityState.Modified;
            _context.SaveChanges();

            TempData["Message"] = "Sửa thành công";

            return RedirectToAction("Index", "Accounts");
        }

        [Route("Delete")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Admin")))
                return RedirectToAction("Login", "Accounts");

            TempData["Message"] = "";

            if (int.TryParse(id, out int adminId))
            {
                var admin = _context.TbQuanTriViens.FirstOrDefault(a => a.Id == adminId);
                if (admin != null)
                {
                    _context.TbQuanTriViens.Remove(admin);
                    _context.SaveChanges();
                    TempData["Message"] = "Xoá thành công";
                }
                else
                {
                    TempData["Message"] = "Không tìm thấy người dùng cần xoá.";
                }
            }
            else
            {
                TempData["Message"] = "ID không hợp lệ";
            }

            return RedirectToAction("Index", "Accounts");
        }
    }
}
