using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Helpers;
using Web_CuaHangCafe.Models;

namespace Web_CuaHangCafe.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CartItem> Carts
        {
            get
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
                if (cart == null)
                {
                    cart = new List<CartItem>();
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                }
                return cart;
            }
            set
            {
                HttpContext.Session.SetObjectAsJson("Cart", value);
            }
        }


        public IActionResult Index()
        {
            var cartItems = Carts;
            ViewData["total"] = cartItems.Sum(p => p.ThanhTien).ToString("n0");
            return View(cartItems);
        }

        public IActionResult Add(int id, int quantity, int size = 0)
        {
            var myCart = Carts;

            // Lấy sản phẩm kèm size
            var product = _context.TbSanPhams
                .Include(p => p.TbSanPhamSizes)
                    .ThenInclude(ps => ps.TbSize)
                .FirstOrDefault(p => p.MaSanPham == id);

            if (product == null)
                return NotFound("Sản phẩm không tồn tại.");

            // Nếu không có size => gán size mặc định là Size có TenSize == "M" hoặc MaSize nhỏ nhất
            if (size == 0)
            {
                var defaultSize = product.TbSanPhamSizes
                    .OrderBy(ps => ps.MaSize)
                    .FirstOrDefault(); // Hoặc dùng .FirstOrDefault(ps => ps.TbSize.TenSize == "M");

                if (defaultSize == null)
                    return NotFound("Không có size nào cho sản phẩm.");

                size = defaultSize.MaSize;
            }

            // Lấy thông tin size từ MaSize
            var productSize = product.TbSanPhamSizes.FirstOrDefault(ps => ps.MaSize == size);
            if (productSize == null)
                return NotFound("Size sản phẩm không hợp lệ.");

            string tenSize = productSize.TbSize.TenSize;
            decimal giaCoBan = product.GiaBan ?? 0m;
            decimal giaTang = productSize.GiaTang ?? 0m;

            // Kiểm tra trùng sản phẩm + size
            var item = myCart.FirstOrDefault(p => p.MaSp == id && p.Size == tenSize);
            if (item == null)
            {
                item = new CartItem
                {
                    MaSp = id,
                    TenSp = product.TenSanPham,
                    AnhSp = product.HinhAnh,
                    DonGiaCoBan = giaCoBan,
                    GiaTang = giaTang,
                    SoLuong = quantity,
                    Size = tenSize,
                    TenSize = tenSize,
                    MaSize = size
                };
                myCart.Add(item);
            }
            else
            {
                item.SoLuong += quantity;
            }

            HttpContext.Session.SetObjectAsJson("Cart", myCart);
            return RedirectToAction("Index");
        }




        [HttpPost]
        public IActionResult Update([FromBody] List<UpdateQuantityRequest> updates)
        {
            if (updates == null || updates.Count == 0)
                return BadRequest(new { success = false, message = "Không có dữ liệu cập nhật." });

            var cartItems = Carts;

            foreach (var update in updates)
            {
                var item = cartItems.FirstOrDefault(p => p.MaSp == update.ProductId && p.Size == update.Size);
                if (item != null)
                {
                    if (update.Quantity <= 0)
                        cartItems.Remove(item);
                    else
                        item.SoLuong = update.Quantity;
                }
            }

            HttpContext.Session.SetObjectAsJson("Cart", cartItems);
            decimal total = cartItems.Sum(p => p.ThanhTien);

            return Json(new
            {
                success = true,
                message = "Cập nhật giỏ hàng thành công.",
                totalAmount = total,
                cartItems
            });
        }

        [HttpPost]
        public IActionResult UpdateCartItem(int productId, string size, int quantity, string note)
        {
            var cartItems = Carts;
            var item = cartItems.FirstOrDefault(p => p.MaSp == productId);
            if (item == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm." });

            // Cập nhật size và số lượng
            item.Size = size;
            item.SoLuong = quantity;
            item.GhiChu = note;

            // Lấy giá từ DB
            var product = _context.TbSanPhams
                .Include(p => p.TbSanPhamSizes)
                .ThenInclude(ps => ps.TbSize)
                .FirstOrDefault(p => p.MaSanPham == productId);

            if (product != null)
            {
                item.DonGiaCoBan = product.GiaBan ?? 0;

                var sizeData = product.TbSanPhamSizes.FirstOrDefault(ps => ps.TbSize.TenSize == size);
                item.GiaTang = sizeData?.GiaTang ?? 0;

                // KHÔNG cần gán DonGia hoặc ThanhTien ở đây nữa
            }

            // Lưu vào session
            Carts = cartItems;

            return Json(new
            {
                success = true,
                totalAmount = cartItems.Sum(p => p.ThanhTien)  // Tự động tính
            });
        }




        [HttpPost]
        public IActionResult Remove(int maSp, string size)
        {
            var cartItems = Carts;
            var item = cartItems.FirstOrDefault(p => p.MaSp == maSp && p.Size == size);

            if (item == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm cần xoá." });

            cartItems.Remove(item);
            HttpContext.Session.SetObjectAsJson("Cart", cartItems);

            return Json(new
            {
                success = true,
                message = "Đã xoá sản phẩm.",
                totalAmount = cartItems.Sum(p => p.ThanhTien),
                cartItems
            });
        }

        public IActionResult Checkout()
        {
            var cartItems = Carts;
            if (cartItems.Count == 0)
                return RedirectToAction("Index", "Home");

            ViewData["total"] = cartItems.Sum(p => p.ThanhTien).ToString("n0");
            return View(cartItems);
        }

        [HttpPost]
        public IActionResult Confirmation(List<CartItem> CartItems, string customerName, string phoneNumber, string address)
        {
            if (CartItems == null || CartItems.Count == 0)
                return RedirectToAction("Index", "Home");

            // 1. Lưu hoặc cập nhật thông tin khách hàng
            var customer = _context.TbKhachHangs.FirstOrDefault(c => c.SdtKhachHang == phoneNumber);
            if (customer == null)
            {
                customer = new TbKhachHang
                {
                    Id = Guid.NewGuid(),
                    TenKhachHang = customerName,
                    SdtKhachHang = phoneNumber,
                    DiaChi = address
                };
                _context.TbKhachHangs.Add(customer);
            }
            else
            {
                customer.TenKhachHang = customerName;
                customer.DiaChi = address;
            }

            _context.SaveChanges();

            // 2. Cập nhật giá Size từ DB để đảm bảo độ chính xác
            var updatedCart = new List<CartItem>();
            foreach (var item in CartItems)
            {
                var product = _context.TbSanPhams
                    .Include(p => p.TbSanPhamSizes)
                        .ThenInclude(ps => ps.TbSize)
                    .FirstOrDefault(p => p.MaSanPham == item.MaSp);

                if (product == null)
                    continue;

                var productSize = product.TbSanPhamSizes.FirstOrDefault(ps => ps.TbSize.TenSize == item.Size);
                if (productSize == null)
                    continue;

                decimal giaCoBan = product.GiaBan ?? 0m;
                decimal giaTang = productSize.GiaTang ?? 0m;

                updatedCart.Add(new CartItem
                {
                    MaSp = item.MaSp,
                    TenSp = product.TenSanPham,
                    Size = item.Size,
                    TenSize = item.Size,
                    SoLuong = item.SoLuong,
                    DonGiaCoBan = giaCoBan,
                    GiaTang = giaTang,
                    MaSize = productSize.MaSize,
                    AnhSp = product.HinhAnh,
                    GhiChu = item.GhiChu
                });
            }

            if (!updatedCart.Any())
                return RedirectToAction("Index", "Home");

            // 3. Lưu hóa đơn
            var orderId = Guid.NewGuid();
            var order = new TbHoaDonBan
            {
                MaHoaDon = orderId,
                SoHoaDon = new Random().Next(100000, 999999).ToString(),
                NgayBan = DateTime.Now,
                TongTien = updatedCart.Sum(p => p.ThanhTien),
                CustomerId = customer.Id
            };
            _context.TbHoaDonBans.Add(order);
            _context.SaveChanges();

            // 4. Lưu chi tiết hóa đơn
            foreach (var item in updatedCart)
            {
                var orderDetail = new TbChiTietHoaDonBan
                {
                    MaHoaDon = orderId,
                    MaSanPham = item.MaSp,
                    GiaBan = item.DonGia,
                    SoLuong = item.SoLuong,
                    ThanhTien = item.ThanhTien,
                    GiamGia = 0,
                    Size = item.Size,
                    GhiChu = item.GhiChu
                };
                _context.TbChiTietHoaDonBans.Add(orderDetail);
            }

            _context.SaveChanges();

            // 5. Xoá session giỏ hàng
            HttpContext.Session.Remove("Cart");

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        public class UpdateQuantityRequest
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public string Size { get; set; } = string.Empty;
        }
    }
}
