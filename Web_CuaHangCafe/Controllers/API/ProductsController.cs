using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Models;

namespace Web_CuaHangCafe.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Lấy tất cả sản phẩm kèm thông tin size
        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.TbSanPhams
                .Include(p => p.MaNhomSpNavigation)
                .OrderBy(p => p.MaSanPham)
                .Select(p => new
                {
                    maSanPham = p.MaSanPham,
                    tenSanPham = p.TenSanPham,
                    nhomSanPham = p.MaNhomSpNavigation.TenNhomSp,
                    giaBan = p.GiaBan,
                    moTa = p.MoTa,
                    hinhAnh = p.HinhAnh,
                    ghiChu = p.GhiChu,
                }).ToList();

            return Ok(products);
        }

        // ✅ Lấy 1 sản phẩm theo ID (chưa bao gồm size)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var product = (from p in _context.TbSanPhams
                               join t in _context.TbNhomSanPhams on p.MaNhomSp equals t.MaNhomSp
                               where p.MaSanPham == id
                               orderby p.MaSanPham
                               select new
                               {
                                   maSanPham = p.MaSanPham,
                                   tenSanPham = p.TenSanPham,
                                   nhomSanPham = t.TenNhomSp,
                                   giaBan = p.GiaBan,
                                   moTa = p.MoTa,
                                   hinhAnh = p.HinhAnh,
                                   ghiChu = p.GhiChu
                               }).ToList();

                if (!product.Any())
                {
                    return NotFound(404);
                }

                return Ok(new
                {
                    success = true,
                    data = product
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // ✅ Tạo mới sản phẩm
        [HttpPost]
        public IActionResult Create(AddProduct product)
        {
            var _product = new TbSanPham
            {
                TenSanPham = product.TenSanPham,
                MaNhomSp = product.MaNhomSp,
                GiaBan = product.GiaBan,
                MoTa = product.MoTa,
                HinhAnh = product.HinhAnh,
                GhiChu = product.GhiChu
            };

            _context.Add(_product);
            _context.SaveChanges();

            var lastProductId = _context.TbSanPhams
                .OrderByDescending(p => p.MaSanPham)
                .Select(p => p.MaSanPham)
                .FirstOrDefault();

            var lastProduct = (from p in _context.TbSanPhams
                               join type in _context.TbNhomSanPhams on p.MaNhomSp equals type.MaNhomSp
                               where p.MaSanPham == lastProductId
                               orderby p.MaSanPham
                               select new
                               {
                                   maSanPham = p.MaSanPham,
                                   tenSanPham = p.TenSanPham,
                                   nhomSanPham = type.TenNhomSp,
                                   giaBan = p.GiaBan,
                                   moTa = p.MoTa,
                                   hinhAnh = p.HinhAnh,
                                   ghiChu = p.GhiChu
                               }).ToList();

            return CreatedAtAction(nameof(GetById), new { id = lastProductId }, lastProduct);
        }

        // ✅ Cập nhật toàn bộ sản phẩm
        [HttpPut("{id}")]
        public IActionResult Edit(int id, AddProduct editProduct)
        {
            try
            {
                var product = _context.TbSanPhams.SingleOrDefault(x => x.MaSanPham == id);

                if (product == null)
                    return NotFound(404);

                product.TenSanPham = editProduct.TenSanPham;
                product.GiaBan = editProduct.GiaBan;
                product.MoTa = editProduct.MoTa;
                product.HinhAnh = editProduct.HinhAnh;
                product.GhiChu = editProduct.GhiChu;
                product.MaNhomSp = editProduct.MaNhomSp;

                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // ✅ Cập nhật một phần
        [HttpPatch("{id}")]
        public IActionResult PartiallEdit(int id, AddProduct editProduct)
        {
            try
            {
                var product = _context.TbSanPhams.SingleOrDefault(x => x.MaSanPham == id);

                if (product == null)
                    return NotFound(404);

                product.TenSanPham = editProduct.TenSanPham;
                product.GiaBan = editProduct.GiaBan;
                product.MoTa = editProduct.MoTa;
                product.HinhAnh = editProduct.HinhAnh;
                product.GhiChu = editProduct.GhiChu;
                product.MaNhomSp = editProduct.MaNhomSp;

                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // ✅ Xoá sản phẩm
        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            try
            {
                var hangHoa = _context.TbSanPhams.SingleOrDefault(x => x.MaSanPham == id);

                if (hangHoa == null)
                    return NotFound(404);

                _context.TbSanPhams.Remove(hangHoa);
                _context.SaveChanges();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
