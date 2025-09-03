using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Clients")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // DANH SÁCH
        [Route("")]
        [Route("Index")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page ?? 1;

            var listItem = _context.TbKhachHangs
                .AsNoTracking()
                .OrderBy(x => x.SdtKhachHang)
                .ToList();

            return View(new PagedList<TbKhachHang>(listItem, pageNumber, pageSize));
        }

        // TÌM KIẾM
        [Route("Search")]
        [HttpGet]
        [Authentication]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page ?? 1;

            if (string.IsNullOrWhiteSpace(search))
                return RedirectToAction("Index");

            search = search.Trim().ToLower();
            ViewBag.search = search;

            var listItem = _context.TbKhachHangs
                .AsNoTracking()
                .Where(x => x.TenKhachHang != null && x.TenKhachHang.ToLower().Contains(search))
                .OrderBy(x => x.SdtKhachHang)
                .ToList();

            return View("Index", new PagedList<TbKhachHang>(listItem, pageNumber, pageSize));
        }

        // TẠO MỚI
        [Route("Create")]
        [HttpGet]
        [Authentication]
        public IActionResult Create()
        {
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authentication]
        public IActionResult Create(TbKhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                khachHang.Id = Guid.NewGuid();
                _context.TbKhachHangs.Add(khachHang);
                _context.SaveChanges();

                TempData["Message"] = "✔️ Thêm khách hàng thành công";
                return RedirectToAction("Index");
            }

            return View(khachHang);
        }

        // CHỈNH SỬA
        [Route("Edit/{id:guid}")]
        [HttpGet]
        [Authentication]
        public IActionResult Edit(Guid id)
        {
            var khachHang = _context.TbKhachHangs.Find(id);
            if (khachHang == null)
                return NotFound();

            ViewBag.name = khachHang.TenKhachHang;
            return View(khachHang);
        }

        [Route("Edit/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authentication]
        public IActionResult Edit(Guid id, TbKhachHang khachHang)
        {
            if (id != khachHang.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.name = khachHang.TenKhachHang;
                return View(khachHang);
            }

            var existing = _context.TbKhachHangs.FirstOrDefault(x => x.Id == khachHang.Id);
            if (existing == null)
                return NotFound();

            existing.TenKhachHang = khachHang.TenKhachHang;
            existing.SdtKhachHang = khachHang.SdtKhachHang;
            existing.DiaChi = khachHang.DiaChi;

            try
            {
                _context.Update(existing);
                _context.SaveChanges();

                TempData["Message"] = "✔️ Cập nhật thông tin khách hàng thành công";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "❌ Lỗi khi cập nhật: " + ex.Message;
                ViewBag.name = khachHang.TenKhachHang;
                return View(khachHang);
            }
        }

        // XOÁ
        [Route("Delete/{id}")]
        [HttpGet]
        [Authentication]
        public IActionResult Delete(string id)
        {
            TempData["Message"] = "";

            if (!Guid.TryParse(id, out Guid guidId))
            {
                TempData["Message"] = "❌ ID khách hàng không hợp lệ.";
                return RedirectToAction("Index");
            }

            var hoaDon = _context.TbHoaDonBans.Where(x => x.CustomerId == guidId).ToList();

            if (hoaDon.Any())
            {
                TempData["Message"] = "⚠️ Không thể xoá vì khách hàng đã có hóa đơn.";
                return RedirectToAction("Index");
            }

            var khachHang = _context.TbKhachHangs.Find(guidId);
            if (khachHang != null)
            {
                _context.TbKhachHangs.Remove(khachHang);
                _context.SaveChanges();
                TempData["Message"] = "🗑️ Xoá khách hàng thành công";
            }
            else
            {
                TempData["Message"] = "❌ Không tìm thấy khách hàng để xoá.";
            }

            return RedirectToAction("Index");
        }
    }
}
