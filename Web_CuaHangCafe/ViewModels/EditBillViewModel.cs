using System;
using System.Collections.Generic;
using Web_CuaHangCafe.Models;

namespace Web_CuaHangCafe.ViewModels
{
    public class EditBillViewModel
    {
        // Thông tin hóa đơn đang được chỉnh sửa
        public TbHoaDonBan HoaDon { get; set; } = new TbHoaDonBan();

        // Danh sách chi tiết món nước đã có trong hóa đơn
        public List<ChiTietHoaDonViewModel> ChiTietHienTai { get; set; } = new List<ChiTietHoaDonViewModel>();

        // Sử dụng để thêm sản phẩm mới vào hóa đơn
        public ChiTietHoaDonViewModel NewProduct { get; set; } = new ChiTietHoaDonViewModel();

        // Danh sách tất cả sản phẩm có thể thêm vào hóa đơn
        public List<TbSanPham> DanhSachSanPham { get; set; } = new List<TbSanPham>();

        // Danh sách khách hàng để chọn khi chỉnh sửa hóa đơn
        public List<TbKhachHang> DanhSachKhachHang { get; set; } = new List<TbKhachHang>();
    }

    public class ChiTietHoaDonViewModel
    {
        // Mã sản phẩm
        public int ProductId { get; set; }

        // Size của món nước (M, L,...)
        public string Size { get; set; } = "";

        // Số lượng sản phẩm
        public int? SoLuong { get; set; }

        // Giá bán mỗi đơn vị sản phẩm
        public decimal? GiaBan { get; set; }
    }
}
