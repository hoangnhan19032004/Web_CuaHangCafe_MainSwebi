using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Feedback")]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("")]
        [Route("Index")]
        //[Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;
            var listItem = _context.TbPhanHois.AsNoTracking().OrderBy(x => x.NgayPhanHoi).ToList();
            PagedList<TbPhanHoi> pagedListItem = new PagedList<TbPhanHoi>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        [Route("Search")]
        //[Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            search = search?.ToLower() ?? "";
            ViewBag.search = search;

            var listItem = _context.TbPhanHois.AsNoTracking()
                .Where(x => x.TieuDe.ToLower().Contains(search))
                .OrderBy(x => x.NgayPhanHoi)
                .ToList();

            PagedList<TbPhanHoi> pagedListItem = new PagedList<TbPhanHoi>(listItem, pageNumber, pageSize);

            return View(pagedListItem);
        }

        [Route("Details")]
        //[Authentication]
        [HttpGet]
        public IActionResult Details(Guid id, string name)
        {
            var phanHoi = _context.TbPhanHois.SingleOrDefault(x => x.Id == id);
            if (phanHoi == null)
            {
                return NotFound();
            }

            ViewBag.name = name;
            return View(phanHoi);
        }

        [Route("Delete")]
        //[Authentication]
        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            TempData["Message"] = "";

            var phanHoi = _context.TbPhanHois.Find(id);
            if (phanHoi != null)
            {
                _context.TbPhanHois.Remove(phanHoi);
                _context.SaveChanges();
                TempData["Message"] = "Xoá thành công";
            }

            return RedirectToAction("Index", "Feedback");
        }
    }
}
