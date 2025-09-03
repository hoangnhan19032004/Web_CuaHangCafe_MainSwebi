namespace Web_CuaHangCafe.ViewModels
{
    public class ProductViewModel
    {
        public int MaSanPham { get; set; }

        public string TenSanPham { get; set; } = null!;

        public decimal? GiaBan { get; set; }

        public decimal? GiaSizeM { get; set; }   // 👈 THÊM
        public decimal? GiaSizeL { get; set; }   // 👈 THÊM

        public string? MoTa { get; set; }

        public string? HinhAnh { get; set; }

        public string? GhiChu { get; set; }

        public string LoaiSanPham { get; set; } = null!;
    }
}
