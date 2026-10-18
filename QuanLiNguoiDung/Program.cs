using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace QuanLiNguoiDung
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            // Kiểm thử nhanh hàm băm mật khẩu ở Bước 5 (in ra cửa sổ Output -> Debug của Visual Studio)
            var kq1 = MatKhau.Tao("123456789");
            var kq2 = MatKhau.Tao("123456789");
            Debug.WriteLine($"[Bước 5] Lần 1 - Salt: {kq1.Salt} | Băm: {kq1.Bam}");
            Debug.WriteLine($"[Bước 5] Lần 2 - Salt: {kq2.Salt} | Băm: {kq2.Bam}");

            // Khởi chạy Form Đăng nhập đầu tiên (Tiêu chí 1)
            Application.Run(new FormDangNhap());
        }
    }
}