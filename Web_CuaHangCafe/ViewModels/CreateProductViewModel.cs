using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web_CuaHangCafe.ViewModels
{
    public class CreateProductViewModel
    {
        public int MaSanPham { get; set; }

        public string TenSanPham { get; set; } = null!;

        public decimal? GiaBan { get; set; }

        public string? MoTa { get; set; }

        public IFormFile? HinhAnh { get; set; } = null!;

        public string? HinhAnhCu { get; set; }

        public string? GhiChu { get; set; }

        public int MaNhomSp { get; set; }  // Khóa ngoại (bắt buộc tồn tại)

        public int MaLoaiSanPham { get; set; }

        // ✅ Thêm dòng này để dùng cho dropdown
        public List<SelectListItem>? DanhSachNhomSp { get; set; }
    }
}
