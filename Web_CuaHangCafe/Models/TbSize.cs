namespace Web_CuaHangCafe.Models
{
    public class TbSize
    {
        public int MaSize { get; set; }
        public string TenSize { get; set; }
        public int HeSoGia { get; set; } // hệ số giá
        public virtual ICollection<TbSanPhamSize> TbSanPhamSizes { get; set; }

    }

}
