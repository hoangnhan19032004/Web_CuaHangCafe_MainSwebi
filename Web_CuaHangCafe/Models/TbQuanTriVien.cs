using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_CuaHangCafe.Models;

public class TbQuanTriVien
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50)]
    public string TenNguoiDung { get; set; }

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(64)] // SHA256 có 64 ký tự hex
    public string MatKhau { get; set; }
}
