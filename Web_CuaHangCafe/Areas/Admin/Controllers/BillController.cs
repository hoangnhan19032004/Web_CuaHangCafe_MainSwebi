using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.ViewModels;
using X.PagedList;
using ClosedXML.Excel;
using System.IO;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Bill")]
    public class BillController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách hóa đơn
        [Route("")]
        [Route("Index")]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page ?? 1;

            var list = _context.TbHoaDonBans
                .Include(h => h.Customer)
                .OrderByDescending(h => h.NgayBan)
                .AsNoTracking()
                .ToList();

            return View(new PagedList<TbHoaDonBan>(list, pageNumber, pageSize));
        }

        // Tìm kiếm hóa đơn theo ngày
        [HttpGet]
        [Route("Search")]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page ?? 1;
            ViewBag.search = search;

            var list = new List<TbHoaDonBan>();

            if (!string.IsNullOrEmpty(search) && DateTime.TryParse(search, out DateTime date))
            {
                list = _context.TbHoaDonBans
                    .Include(h => h.Customer)
                    .Where(h => h.NgayBan.HasValue && h.NgayBan.Value.Date == date.Date)
                    .OrderByDescending(h => h.NgayBan)
                    .AsNoTracking()
                    .ToList();
            }

            return View("Index", new PagedList<TbHoaDonBan>(list, pageNumber, pageSize));
        }

        // Xuất hóa đơn ra Excel
        [HttpGet]
        [Route("ExportToExcel")]
        public IActionResult ExportToExcel(string search)
        {
            var query = _context.TbHoaDonBans.Include(h => h.Customer).AsQueryable();

            if (!string.IsNullOrEmpty(search) && DateTime.TryParse(search, out var date))
            {
                query = query.Where(h => h.NgayBan.HasValue && h.NgayBan.Value.Date == date.Date);
            }

            var list = query.OrderByDescending(h => h.NgayBan).ToList();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("HoaDonBan");

            worksheet.Cell(1, 1).Value = "Mã hóa đơn";
            worksheet.Cell(1, 2).Value = "Ngày bán";
            worksheet.Cell(1, 3).Value = "Khách hàng";
            worksheet.Cell(1, 4).Value = "Tổng tiền";

            for (int i = 0; i < list.Count; i++)
            {
                var row = i + 2;
                worksheet.Cell(row, 1).Value = list[i].SoHoaDon;
                worksheet.Cell(row, 2).Value = list[i].NgayBan?.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 3).Value = list[i].Customer?.TenKhachHang ?? "Không xác định";
                worksheet.Cell(row, 4).Value = list[i].TongTien ?? 0;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0 \"VNĐ\"";
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"HoaDonBan_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        // Xem chi tiết hóa đơn
        [HttpGet]
        [Route("Details")]
        public IActionResult Details(int? page, string id, string name)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            int pageSize = 30;
            int pageNumber = page ?? 1;

            var details = _context.TbChiTietHoaDonBans
                .Where(ct => ct.MaHoaDon == Guid.Parse(id))
                .Include(ct => ct.MaSanPhamNavigation)
                .OrderBy(ct => ct.MaSanPham)
                .AsNoTracking()
                .ToList();

            ViewBag.name = name;
            return View(new PagedList<TbChiTietHoaDonBan>(details, pageNumber, pageSize));
        }

        // GET: Chỉnh sửa hóa đơn
        [HttpGet]
        [Route("Edit/{id}")]
        public IActionResult Edit(Guid id)
        {
            var bill = _context.TbHoaDonBans
                .Include(h => h.Customer)
                .Include(h => h.TbChiTietHoaDonBans)
                    .ThenInclude(ct => ct.MaSanPhamNavigation)
                .FirstOrDefault(h => h.MaHoaDon == id);

            if (bill == null) return NotFound();

            var viewModel = new EditBillViewModel
            {
                HoaDon = bill,
                ChiTietHienTai = bill.TbChiTietHoaDonBans.Select(ct => new ChiTietHoaDonViewModel
                {
                    ProductId = ct.MaSanPham,
                    Size = ct.Size ?? "",
                    SoLuong = ct.SoLuong ?? 0,
                    GiaBan = ct.GiaBan ?? 0
                }).ToList(),
                DanhSachSanPham = _context.TbSanPhams.ToList(),
                DanhSachKhachHang = _context.TbKhachHangs.ToList()
            };

            return View(viewModel);
        }

        // POST: Xử lý thêm / xóa / cập nhật
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public IActionResult Edit(Guid id, EditBillViewModel model, string action, int? removeIndex, List<int>? UpdatedQuantities)
        {
            if (id != model.HoaDon.MaHoaDon) return NotFound();

            var bill = _context.TbHoaDonBans
                .Include(h => h.TbChiTietHoaDonBans)
                .FirstOrDefault(h => h.MaHoaDon == id);

            if (bill == null) return NotFound();

            // Xóa món
            if (removeIndex.HasValue && removeIndex.Value >= 0 && removeIndex.Value < bill.TbChiTietHoaDonBans.Count)
            {
                var detailToRemove = bill.TbChiTietHoaDonBans.ElementAt(removeIndex.Value);
                _context.TbChiTietHoaDonBans.Remove(detailToRemove);
            }

            // Thêm món mới
            if (action == "add" && model.NewProduct != null && model.NewProduct.ProductId > 0)
            {
                // Kiểm tra trùng sản phẩm + size trong hóa đơn
                bool isDuplicate = bill.TbChiTietHoaDonBans
                    .Any(ct => ct.MaSanPham == model.NewProduct.ProductId && ct.Size == model.NewProduct.Size);

                if (isDuplicate)
                {
                    TempData["error"] = "Món này đã có trong hóa đơn, vui lòng chọn món khác!";
                    return RedirectToAction(nameof(Edit), new { id });
                }

                // Lấy thông tin sản phẩm để tính giá bán
                var sanPham = _context.TbSanPhams.FirstOrDefault(sp => sp.MaSanPham == model.NewProduct.ProductId);
                if (sanPham != null)
                {
                    decimal giaBan = sanPham.GiaBan ?? 0;
                    if (model.NewProduct.Size == "L") giaBan += 10000;

                    var newDetail = new TbChiTietHoaDonBan
                    {
                        MaHoaDon = bill.MaHoaDon,
                        MaSanPham = model.NewProduct.ProductId,
                        Size = model.NewProduct.Size,
                        SoLuong = model.NewProduct.SoLuong,
                        GiaBan = giaBan
                    };
                    _context.TbChiTietHoaDonBans.Add(newDetail);
                }
            }

            // Cập nhật số lượng
            if (UpdatedQuantities != null)
            {
                for (int i = 0; i < bill.TbChiTietHoaDonBans.Count; i++)
                {
                    if (i < UpdatedQuantities.Count)
                    {
                        bill.TbChiTietHoaDonBans.ElementAt(i).SoLuong = UpdatedQuantities[i];
                    }
                }
            }

            // Cập nhật tổng tiền
            bill.TongTien = bill.TbChiTietHoaDonBans.Sum(ct => (ct.GiaBan ?? 0) * (ct.SoLuong ?? 0));

            _context.SaveChanges();

            // Nếu bấm Lưu hóa đơn thì quay về Index
            if (action == "save")
            {
                TempData["success"] = "Cập nhật hóa đơn thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Còn lại (thêm, xóa, cập nhật số lượng) thì ở lại trang Edit
            return RedirectToAction(nameof(Edit), new { id });
        }

        // GET: Xóa hóa đơn
        [HttpGet]
        [Route("Delete/{id}")]
        public IActionResult Delete(Guid id)
        {
            var bill = _context.TbHoaDonBans
                .Include(h => h.Customer)
                .FirstOrDefault(h => h.MaHoaDon == id);

            if (bill == null) return NotFound();

            return View(bill);
        }

        // POST: Xóa hóa đơn
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete/{id}")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var bill = _context.TbHoaDonBans
                .Include(h => h.TbChiTietHoaDonBans)
                .FirstOrDefault(h => h.MaHoaDon == id);

            if (bill == null) return NotFound();

            _context.TbChiTietHoaDonBans.RemoveRange(bill.TbChiTietHoaDonBans);
            _context.TbHoaDonBans.Remove(bill);
            _context.SaveChanges();

            TempData["success"] = "Xóa hóa đơn thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
