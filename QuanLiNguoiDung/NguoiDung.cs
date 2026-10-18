using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLiNguoiDung
{
    public class NguoiDung
    {
        public int UserId { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string HoTen { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? LanDangNhapCuoi { get; set; }
    }
}
