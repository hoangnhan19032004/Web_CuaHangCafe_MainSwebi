namespace Web_CuaHangCafe.Models
{
    public class CartItem
    {
        public int MaSp { get; set; }
        public string TenSp { get; set; } = null!;
        public string AnhSp { get; set; } = null!;

        public decimal DonGiaCoBan { get; set; }       // Giá cơ bản
        public decimal GiaTang { get; set; }           // Giá tăng thêm do size
        public decimal DonGia => DonGiaCoBan + GiaTang; // Tổng giá đã tính size

        public int SoLuong { get; set; }
        public string Size { get; set; } = "M";
        public int MaSize { get; set; }
        public string TenSize { get; set; } = "";
        public string? GhiChu { get; set; }

        public decimal ThanhTien => DonGia * SoLuong;
    }
}
