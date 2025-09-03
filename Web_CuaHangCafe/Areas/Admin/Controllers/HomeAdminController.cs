using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;
using Web_CuaHangCafe.Models.Authentication;
using Web_CuaHangCafe.ViewModels;
using X.PagedList;

namespace Web_CuaHangCafe.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("HomeAdmin")]
    public class HomeAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment hostEnvironment;

        public HomeAdminController(ApplicationDbContext context, IWebHostEnvironment hc)
        {
            _context = context;
            hostEnvironment = hc;
        }

        [Route("")]
        [Authentication]
        public IActionResult Index(int? page)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            var listItem = (from product in _context.TbSanPhams
                            join type in _context.TbNhomSanPhams on product.MaNhomSp equals type.MaNhomSp
                            orderby product.MaSanPham
                            select new ProductViewModel
                            {
                                MaSanPham = product.MaSanPham,
                                TenSanPham = product.TenSanPham,
                                GiaBan = product.GiaBan,
                                MoTa = product.MoTa,
                                HinhAnh = product.HinhAnh,
                                GhiChu = product.GhiChu,
                                LoaiSanPham = type.TenNhomSp
                            }).ToList();

            return View(new PagedList<ProductViewModel>(listItem, pageNumber, pageSize));
        }

        [Route("Search")]
        [Authentication]
        [HttpGet]
        public IActionResult Search(int? page, string search)
        {
            int pageSize = 30;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            search = search?.ToLower() ?? "";
            ViewBag.search = search;

            var listItem = _context.TbSanPhams
                .Where(x => x.TenSanPham.ToLower().Contains(search))
                .OrderBy(x => x.MaSanPham)
                .ToList();

            return View(new PagedList<TbSanPham>(listItem, pageNumber, pageSize));
        }

        [Route("Create")]
        [Authentication]
        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new CreateProductViewModel
            {
                DanhSachNhomSp = _context.TbNhomSanPhams
                    .Select(nsp => new SelectListItem
                    {
                        Value = nsp.MaNhomSp.ToString(),
                        Text = nsp.TenNhomSp
                    }).ToList()
            };

            return View(viewModel);
        }

        [Route("Create")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateProductViewModel createProduct)
        {
            if (!ModelState.IsValid)
            {
                createProduct.DanhSachNhomSp = _context.TbNhomSanPhams
                    .Select(nsp => new SelectListItem
                    {
                        Value = nsp.MaNhomSp.ToString(),
                        Text = nsp.TenNhomSp
                    }).ToList();

                return View(createProduct);
            }

            string fileName = null;

            if (createProduct.HinhAnh != null && createProduct.HinhAnh.Length > 0)
            {
                string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, "img", "products");
                fileName = Guid.NewGuid().ToString() + "_" + createProduct.HinhAnh.FileName;
                string filePath = Path.Combine(uploadFolder, fileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    createProduct.HinhAnh.CopyTo(fileStream);
                }
            }

            var product = new TbSanPham
            {
                TenSanPham = createProduct.TenSanPham,
                GiaBan = createProduct.GiaBan,
                MoTa = createProduct.MoTa,
                HinhAnh = fileName,
                GhiChu = createProduct.GhiChu,
                MaNhomSp = createProduct.MaNhomSp
            };

            _context.TbSanPhams.Add(product);
            _context.SaveChanges();
            TempData["Message"] = "Thêm sản phẩm thành công";

            return RedirectToAction("Index");
        }

        [Route("Details")]
        [Authentication]
        [HttpGet]
        public IActionResult Details(int id, string name)
        {
            var productItem = (from product in _context.TbSanPhams
                               join type in _context.TbNhomSanPhams on product.MaNhomSp equals type.MaNhomSp
                               where product.MaSanPham == id
                               select new ProductViewModel
                               {
                                   MaSanPham = product.MaSanPham,
                                   TenSanPham = product.TenSanPham,
                                   GiaBan = product.GiaBan,
                                   MoTa = product.MoTa,
                                   HinhAnh = product.HinhAnh,
                                   GhiChu = product.GhiChu,
                                   LoaiSanPham = type.TenNhomSp
                               }).SingleOrDefault();

            ViewBag.name = name;
            return View(productItem);
        }

        [Route("Edit")]
        [Authentication]
        [HttpGet]
        public IActionResult Edit(int id, string name)
        {
            var product = _context.TbSanPhams.Find(id);
            if (product == null) return NotFound();

            var viewModel = new CreateProductViewModel
            {
                MaSanPham = product.MaSanPham,
                TenSanPham = product.TenSanPham,
                GiaBan = product.GiaBan,
                MoTa = product.MoTa,
                GhiChu = product.GhiChu,
                HinhAnhCu = product.HinhAnh,
                MaNhomSp = product.MaNhomSp,
                DanhSachNhomSp = _context.TbNhomSanPhams
                    .Select(nsp => new SelectListItem
                    {
                        Value = nsp.MaNhomSp.ToString(),
                        Text = nsp.TenNhomSp
                    }).ToList()
            };

            ViewBag.name = name;
            return View(viewModel);
        }

        [Route("Edit")]
        [Authentication]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateProductViewModel createProduct)
        {
            var product = _context.TbSanPhams.Find(createProduct.MaSanPham);
            if (product == null) return NotFound();

            product.TenSanPham = createProduct.TenSanPham;
            product.GiaBan = createProduct.GiaBan;
            product.MoTa = createProduct.MoTa;
            product.GhiChu = createProduct.GhiChu;
            product.MaNhomSp = createProduct.MaNhomSp;

            if (createProduct.HinhAnh != null && createProduct.HinhAnh.Length > 0)
            {
                string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, "img", "products");
                string fileName = Guid.NewGuid().ToString() + "_" + createProduct.HinhAnh.FileName;
                string filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    createProduct.HinhAnh.CopyTo(stream);
                }

                product.HinhAnh = fileName;
            }
            else
            {
                product.HinhAnh = createProduct.HinhAnhCu;
            }

            _context.Update(product);
            _context.SaveChanges();

            TempData["Message"] = "Sửa sản phẩm thành công";
            return RedirectToAction("Index");
        }

        [Route("Delete")]
        [Authentication]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            TempData["Message"] = "";

            var chiTietHoaDon = _context.TbChiTietHoaDonBans.Where(x => x.MaSanPham == id).ToList();
            if (chiTietHoaDon.Any())
            {
                TempData["Message"] = "Không xoá được sản phẩm";
                return RedirectToAction("Index");
            }

            var sanPham = _context.TbSanPhams.Find(id);
            if (sanPham != null)
            {
                _context.TbSanPhams.Remove(sanPham);
                _context.SaveChanges();
                TempData["Message"] = "Sản phẩm đã được xoá";
            }

            return RedirectToAction("Index");
        }
    }
}
