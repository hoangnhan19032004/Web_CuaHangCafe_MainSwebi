using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web_CuaHangCafe.Models;

public class TbPhanHoi
{
    [Key]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
    public required string TieuDe { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public required string SoDienThoai { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập lời nhắn")]
    public required string NoiDung { get; set; }

    public DateTime NgayPhanHoi { get; set; }
}


