using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/News")]
    public class NewsManageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsManageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            var listItem = _context.TbTinTucs
                                   .AsNoTracking()
                                   .OrderBy(x => x.MaTinTuc)
                                   .ToList();

            var pagedListItem = new PagedList<TbTinTuc>(listItem, pageNumber, pageSize);
            return View(pagedListItem);
        }

        [Route("Search")]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 1 ? 1 : page.Value;

            // ✅ Fix lỗi search null
            search = string.IsNullOrEmpty(search) ? "" : search.ToLower();
            ViewBag.search = search;

            var listItem = _context.TbTinTucs
                                   .AsNoTracking()
                                   .Where(x => x.TieuDe.ToLower().Contains(search))
                                   .OrderBy(x => x.MaTinTuc)
                                   .ToList();

            var pagedListItem = new PagedList<TbTinTuc>(listItem, pageNumber, pageSize);
            return View("Index", pagedListItem); // Dùng lại view Index
        }

        [Route("Create")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.NguoiDang = new SelectList(_context.TbQuanTriViens.ToList(), "TenNguoiDung", "TenNguoiDung");
            return View();
        }

        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TbTinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                _context.TbTinTucs.Add(tinTuc);
                _context.SaveChanges();

                TempData["Message"] = "Thêm thành công";
                return RedirectToAction("Index");
            }

            ViewBag.NguoiDang = new SelectList(_context.TbQuanTriViens.ToList(), "TenNguoiDung", "TenNguoiDung");
            return View(tinTuc);
        }

        [Route("Details")]
        [HttpGet]
        public IActionResult Details(int id, string name)
        {
            var tinTuc = _context.TbTinTucs.FirstOrDefault(x => x.MaTinTuc == id);
            if (tinTuc == null)
            {
                return NotFound();
            }

            ViewBag.name = name;
            return View(tinTuc);
        }

        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var tinTuc = _context.TbTinTucs.Find(id);
            if (tinTuc == null)
            {
                return NotFound();
            }

            ViewBag.NguoiDang = new SelectList(_context.TbQuanTriViens.ToList(), "TenNguoiDung", "TenNguoiDung");
            ViewBag.name = name;

            return View(tinTuc);
        }

        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(TbTinTuc tinTuc)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(tinTuc).State = EntityState.Modified;
                _context.SaveChanges();

                TempData["Message"] = "Sửa thành công";
                return RedirectToAction("Index");
            }

            ViewBag.NguoiDang = new SelectList(_context.TbQuanTriViens.ToList(), "TenNguoiDung", "TenNguoiDung");
            return View(tinTuc);
        }

        [Route("Delete")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var tinTuc = _context.TbTinTucs.Find(id);
            if (tinTuc == null)
            {
                TempData["Message"] = "Không tìm thấy tin tức để xoá.";
                return RedirectToAction("Index");
            }

            _context.TbTinTucs.Remove(tinTuc);
            _context.SaveChanges();

            TempData["Message"] = "Xoá thành công";
            return RedirectToAction("Index");
        }
    }
}
