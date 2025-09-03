namespace Web_CuaHangCafe.Models
{
    public partial class TbSanPhamSize
    {
        public int MaSanPham { get; set; }
        public int MaSize { get; set; }
        public decimal? GiaTang { get; set; } // Giá tăng cho size riêng cho sản phẩm

        public virtual TbSanPham TbSanPham { get; set; }
        public virtual TbSize TbSize { get; set; }
    }

}
