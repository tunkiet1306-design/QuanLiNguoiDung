using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuanLiNguoiDung
{
    public partial class FormDangKy : Form
    {
        private TextBox txtTenDangNhap = null!;
        private TextBox txtMatKhau = null!;
        private TextBox txtXacNhan = null!;
        private TextBox txtHoTen = null!;
        private TextBox txtEmail = null!;

        // Nhãn lỗi riêng cho từng ô nhập (luôn hiện diện với Text = "")
        private Label lblLoiTenDangNhap = null!;
        private Label lblLoiMatKhau = null!;
        private Label lblLoiXacNhan = null!;
        private Label lblLoiHoTen = null!;
        private Label lblLoiEmail = null!;
        private Label lblLoiChung = null!;

        private Button btnDangKy = null!;
        private Button btnHuy = null!;

        private readonly NguoiDungRepository _repo = new NguoiDungRepository();

        private readonly Color MauNenForm = Color.FromArgb(241, 245, 249);
        private readonly Color MauTrangCard = Color.White;
        private readonly Color MauChuDao = Color.FromArgb(37, 99, 235);
        private readonly Color MauChuChinh = Color.FromArgb(15, 23, 42);
        private readonly Color MauChuPhu = Color.FromArgb(100, 116, 139);
        private readonly Color MauLoi = Color.FromArgb(220, 38, 38);

        public string TenDangNhapVuaTao { get; private set; } = string.Empty;
        public FormDangKy()
        {
            KhoiTaoGiaoDien();
        }

        private int ScalePx(int px) => (int)Math.Round(px * (this.DeviceDpi / 96f));
        private void KhoiTaoGiaoDien()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            this.Text = "Hệ thống Quản lý Người dùng - Đăng ký tài khoản";
            this.BackColor = MauNenForm;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(ScalePx(24));

            // Khung Card trắng trung tâm
            Panel pnlCard = new Panel
            {
                BackColor = MauTrangCard,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                MinimumSize = new Size(ScalePx(460), 0)
            };

            Panel pnlAccent = new Panel
            {
                Dock = DockStyle.Top,
                Height = ScalePx(6),
                BackColor = MauChuDao
            };
            pnlCard.Controls.Add(pnlAccent);

            TableLayoutPanel tlpMain = new TableLayoutPanel
            {
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(ScalePx(28), ScalePx(20), ScalePx(28), ScalePx(22)),
                MinimumSize = new Size(ScalePx(460), 0)
            };
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Tiêu đề Form Đăng ký
            Label lblTieuDe = new Label
            {
                Text = "Tạo tài khoản mới",
                Font = new Font("Segoe UI", 17F, FontStyle.Bold),
                ForeColor = MauChuChinh,
                AutoSize = true,
                Margin = new Padding(0, ScalePx(2), 0, ScalePx(2))
            };
            Label lblPhuDe = new Label
            {
                Text = "Điền đầy đủ các thông tin bên dưới để đăng ký tài khoản",
                Font = new Font("Segoe UI", 9F),
                ForeColor = MauChuPhu,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, ScalePx(14))
            };
            tlpMain.Controls.Add(lblTieuDe);
            tlpMain.Controls.Add(lblPhuDe);

            // 1. Tên đăng nhập
            ThemTruongDon(tlpMain, "Tên đăng nhập *", "txtTenDangNhap", "3-20 ký tự (chữ, số, dấu _)",
                false, 0, out txtTenDangNhap, out lblLoiTenDangNhap);
            txtTenDangNhap.MaxLength = 20;

            // 2. Hàng đôi: Mật khẩu & Xác nhận mật khẩu nằm trên 2 cột cân đối
            TableLayoutPanel tlpMatKhau = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0)
            };
            tlpMatKhau.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpMatKhau.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            Label lblMK = TaoNhanTruong("Mật khẩu *");
            Label lblXN = TaoNhanTruong("Xác nhận mật khẩu *");
            lblXN.Margin = new Padding(ScalePx(8), 0, 0, ScalePx(4));

            txtMatKhau = new TextBox
            {
                Name = "txtMatKhau",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true,
                PlaceholderText = "Tối thiểu 8 ký tự",
                Margin = new Padding(0, 0, ScalePx(8), ScalePx(2)),
                TabIndex = 1
            };

            txtXacNhan = new TextBox
            {
                Name = "txtXacNhan",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                UseSystemPasswordChar = true,
                PlaceholderText = "Nhập lại mật khẩu",
                Margin = new Padding(ScalePx(8), 0, 0, ScalePx(2)),
                TabIndex = 2
            };

            lblLoiMatKhau = TaoNhanLoi("lblLoi_txtMatKhau");
            lblLoiXacNhan = TaoNhanLoi("lblLoi_txtXacNhan");
            lblLoiXacNhan.Margin = new Padding(ScalePx(8), 0, 0, ScalePx(4));

            tlpMatKhau.Controls.Add(lblMK, 0, 0);
            tlpMatKhau.Controls.Add(lblXN, 1, 0);
            tlpMatKhau.Controls.Add(txtMatKhau, 0, 1);
            tlpMatKhau.Controls.Add(txtXacNhan, 1, 1);
            tlpMatKhau.Controls.Add(lblLoiMatKhau, 0, 2);
            tlpMatKhau.Controls.Add(lblLoiXacNhan, 1, 2);
            tlpMain.Controls.Add(tlpMatKhau);

            // 3. Họ và tên
            ThemTruongDon(tlpMain, "Họ và tên *", "txtHoTen", "Ví dụ: Nguyễn Văn A",
                false, 3, out txtHoTen, out lblLoiHoTen);
            txtHoTen.MaxLength = 50;

            // 4. Email
            ThemTruongDon(tlpMain, "Địa chỉ Email *", "txtEmail", "Ví dụ: ten@mien.com",
                false, 4, out txtEmail, out lblLoiEmail);
            txtEmail.MaxLength = 100;

            // Nhãn lỗi kết nối chung
            lblLoiChung = new Label
            {
                Name = "lblLoiChung",
                Text = "",
                ForeColor = MauLoi,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                AutoSize = true,
                MinimumSize = new Size(0, ScalePx(22)),
                Margin = new Padding(0, 0, 0, ScalePx(6))
            };
            tlpMain.Controls.Add(lblLoiChung);

            // 5. Cụm nút Đăng ký & Huỷ
            TableLayoutPanel tlpButtons = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, ScalePx(2), 0, 0)
            };
            tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));

            btnDangKy = new Button
            {
                Name = "btnDangKy",
                Text = "Đăng ký tài khoản",
                Dock = DockStyle.Fill,
                MinimumSize = new Size(0, ScalePx(40)),
                AutoSize = true,
                BackColor = MauChuDao,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, ScalePx(8), 0),
                TabIndex = 5
            };
            btnDangKy.FlatAppearance.BorderSize = 0;
            btnDangKy.Click += BtnDangKy_Click;

            btnHuy = new Button
            {
                Name = "btnHuy",
                Text = "Huỷ bỏ",
                Dock = DockStyle.Fill,
                MinimumSize = new Size(0, ScalePx(40)),
                AutoSize = true,
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = MauChuChinh,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0),
                TabIndex = 6
            };
            btnHuy.FlatAppearance.BorderSize = 0;
            btnHuy.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            tlpButtons.Controls.Add(btnDangKy, 0, 0);
            tlpButtons.Controls.Add(btnHuy, 1, 0);
            tlpMain.Controls.Add(tlpButtons);

            pnlCard.Controls.Add(tlpMain);
            this.Controls.Add(pnlCard);

            this.AcceptButton = btnDangKy;
            this.CancelButton = btnHuy;
        }

        private Label TaoNhanTruong(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = MauChuChinh,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, ScalePx(4))
            };
        }

        private Label TaoNhanLoi(string name)
        {
            return new Label
            {
                Name = name,
                Text = "",
                ForeColor = MauLoi,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                Dock = DockStyle.Fill,
                AutoSize = true,
                MinimumSize = new Size(0, ScalePx(20)),
                Margin = new Padding(0, 0, 0, ScalePx(4))
            };
        }

        private void ThemTruongDon(TableLayoutPanel parent, string nhanText, string tenControl, string goiY,
            bool laMatKhau, int tabIndex, out TextBox textBox, out Label lblLoi)
        {
            Label lbl = TaoNhanTruong(nhanText);

            textBox = new TextBox
            {
                Name = tenControl,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                PlaceholderText = goiY,
                UseSystemPasswordChar = laMatKhau,
                Margin = new Padding(0, 0, 0, ScalePx(2)),
                TabIndex = tabIndex
            };

            lblLoi = TaoNhanLoi("lblLoi_" + tenControl);

            parent.Controls.Add(lbl);
            parent.Controls.Add(textBox);
            parent.Controls.Add(lblLoi);
        }

        private void XoaThongBaoLoi()
        {
            lblLoiTenDangNhap.Text = "";
            lblLoiMatKhau.Text = "";
            lblLoiXacNhan.Text = "";
            lblLoiHoTen.Text = "";
            lblLoiEmail.Text = "";
            lblLoiChung.Text = "";
        }

        // Hàm báo lỗi gọn theo đúng mẫu Mục III.3 trong Báo cáo 2.1
        private static bool Loi(Label lbl, Control o, string thongBao)
        {
            lbl.Text = thongBao;
            o.Focus();
            return false;
        }

        private bool KiemTraHopLeDuLieu()
        {
            XoaThongBaoLoi();

            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;
            string xacNhan = txtXacNhan.Text;
            string hoTen = txtHoTen.Text.Trim();
            string email = txtEmail.Text.Trim();

            // 1. Kiểm tra Tên đăng nhập (Theo Bảng II.2 Bài 2.1)
            if (string.IsNullOrEmpty(tenDangNhap))
                return Loi(lblLoiTenDangNhap, txtTenDangNhap, "Vui lòng nhập tên đăng nhập.");

            if (tenDangNhap.Length < 3 || tenDangNhap.Length > 20)
                return Loi(lblLoiTenDangNhap, txtTenDangNhap, "Tên đăng nhập phải có từ 3 đến 20 ký tự.");

            if (!Regex.IsMatch(tenDangNhap, @"^[a-zA-Z0-9_]+$"))
                return Loi(lblLoiTenDangNhap, txtTenDangNhap, "Chỉ được sử dụng chữ cái không dấu, chữ số và dấu gạch dưới.");

            // 2. Kiểm tra Mật khẩu
            if (string.IsNullOrEmpty(matKhau))
                return Loi(lblLoiMatKhau, txtMatKhau, "Vui lòng nhập mật khẩu.");

            if (matKhau.Length < 8)
                return Loi(lblLoiMatKhau, txtMatKhau, "Mật khẩu phải có ít nhất 8 ký tự.");

            if (!matKhau.Any(char.IsLetter) || !matKhau.Any(char.IsDigit))
                return Loi(lblLoiMatKhau, txtMatKhau, "Mật khẩu phải chứa ít nhất một chữ cái và một chữ số.");

            // 3. Kiểm tra Xác nhận mật khẩu
            if (string.IsNullOrEmpty(xacNhan))
                return Loi(lblLoiXacNhan, txtXacNhan, "Vui lòng xác nhận mật khẩu.");

            if (matKhau != xacNhan)
                return Loi(lblLoiXacNhan, txtXacNhan, "Mật khẩu xác nhận không khớp.");

            // 4. Kiểm tra Họ và tên
            if (string.IsNullOrEmpty(hoTen))
                return Loi(lblLoiHoTen, txtHoTen, "Vui lòng nhập họ và tên.");

            if (hoTen.Length < 2 || hoTen.Length > 50)
                return Loi(lblLoiHoTen, txtHoTen, "Họ và tên phải có từ 2 đến 50 ký tự.");

            // 5. Kiểm tra Email (Bắt buộc NOT NULL & Đúng định dạng theo Bảng II.2 Bài 2.1)
            if (string.IsNullOrEmpty(email))
                return Loi(lblLoiEmail, txtEmail, "Vui lòng nhập email để phục vụ khôi phục mật khẩu.");

            if (email.Length > 100 || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return Loi(lblLoiEmail, txtEmail, "Địa chỉ email không hợp lệ. Ví dụ: ten@mien.com.");

            return true;
        }

        private async void BtnDangKy_Click(object? sender, EventArgs e)
        {
            if (!KiemTraHopLeDuLieu())
            {
                return;
            }

            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;
            string hoTen = txtHoTen.Text.Trim();
            string email = txtEmail.Text.Trim();

            btnDangKy.Enabled = false;
            btnDangKy.Text = "Đang xử lý...";

            try
            {
                var (salt, matKhauBam) = MatKhau.Tao(matKhau);

                // Bước 1: Kiểm tra trùng Tên đăng nhập trong CSDL (Select Username)
                if (await _repo.KiemTraTonTaiTenDangNhapAsync(tenDangNhap))
                {
                    Loi(lblLoiTenDangNhap, txtTenDangNhap, "Tên này đã có người dùng");
                    return;
                }

                // Bước 2: Kiểm tra trùng Email trong CSDL (Select Email theo Sequence Diagram Bài 2.1)
                if (await _repo.KiemTraTonTaiEmailAsync(email))
                {
                    Loi(lblLoiEmail, txtEmail, "Email đã được sử dụng cho một tài khoản khác.");
                    return;
                }

                // Bước 3: Thêm người dùng mới (Insert User)
                await _repo.ThemNguoiDungAsync(tenDangNhap, matKhauBam, salt, hoTen, email);

                MessageBox.Show("Đăng ký tài khoản thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                TenDangNhapVuaTao = tenDangNhap;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // Xử lý khi 2 Client đăng ký cùng lúc (Tiêu chí DK-13): Phân biệt trùng Username hay trùng Email
                try
                {
                    if (await _repo.KiemTraTonTaiTenDangNhapAsync(tenDangNhap))
                    {
                        Loi(lblLoiTenDangNhap, txtTenDangNhap, "Tên này đã có người dùng");
                        return;
                    }
                    if (await _repo.KiemTraTonTaiEmailAsync(email))
                    {
                        Loi(lblLoiEmail, txtEmail, "Email đã được sử dụng cho một tài khoản khác.");
                        return;
                    }
                }
                catch
                {
                    // Fallback nếu mất kết nối ngay lúc kiểm tra lại
                }

                Loi(lblLoiChung, txtTenDangNhap, "Tên đăng nhập hoặc Email đã được sử dụng.");
            }
            catch (SqlException)
            {
                lblLoiChung.Text = "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau.";
            }
            catch (Exception)
            {
                lblLoiChung.Text = "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau.";
            }
            finally
            {
                btnDangKy.Enabled = true;
                btnDangKy.Text = "Đăng ký tài khoản";
            }
        }
    }
}
