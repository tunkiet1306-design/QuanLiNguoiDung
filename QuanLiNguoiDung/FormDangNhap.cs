using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace QuanLiNguoiDung
{
    public partial class FormDangNhap : Form
    {
        private TextBox txtTenDangNhap = null!;
        private TextBox txtMatKhau = null!;
        private Label lblThongBaoLoi = null!;
        private Button btnDangNhap = null!;
        private Button btnThoat = null!;
        private LinkLabel lnkDangKy = null!;

        private readonly NguoiDungRepository _repo = new NguoiDungRepository();

        // Bảng màu Modern Slate & Indigo
        private readonly Color MauNenForm = Color.FromArgb(241, 245, 249);
        private readonly Color MauTrangCard = Color.White;
        private readonly Color MauChuDao = Color.FromArgb(37, 99, 235);     // Xanh Indigo
        private readonly Color MauChuChinh = Color.FromArgb(15, 23, 42);    // Đen Slate đậm
        private readonly Color MauChuPhu = Color.FromArgb(100, 116, 139);   // Xám Slate
        private readonly Color MauLoi = Color.FromArgb(220, 38, 38);        // Đỏ cảnh báo
        public FormDangNhap()
        {
            KhoiTaoGiaoDien();
        }

        private int ScalePx(int px) => (int)Math.Round(px * (this.DeviceDpi / 96f));
        private void KhoiTaoGiaoDien()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.Text = "Hệ thống Quản lý Người dùng - Đăng nhập";
            this.BackColor = MauNenForm;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Tự động co giãn kích thước cửa sổ ôm khít nội dung theo DPI của từng máy
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(ScalePx(28));

            // Khung Card trắng trung tâm
            Panel pnlCard = new Panel
            {
                BackColor = MauTrangCard,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                MinimumSize = new Size(ScalePx(380), 0)
            };

            // Thanh màu điểm nhấn trên đỉnh Card
            Panel pnlAccent = new Panel
            {
                Dock = DockStyle.Top,
                Height = ScalePx(6),
                BackColor = MauChuDao
            };
            pnlCard.Controls.Add(pnlAccent);

            // Bố cục lưới xếp dọc tự động giãn theo DPI
            TableLayoutPanel tlpMain = new TableLayoutPanel
            {
                ColumnCount = 1,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(ScalePx(28), ScalePx(24), ScalePx(28), ScalePx(24)),
                MinimumSize = new Size(ScalePx(380), 0)
            };
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // 1. Tiêu đề chính & Phụ đề
            Label lblTieuDe = new Label
            {
                Text = "Đăng nhập",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = MauChuChinh,
                AutoSize = true,
                Margin = new Padding(0, ScalePx(4), 0, ScalePx(2))
            };
            Label lblPhuDe = new Label
            {
                Text = "Vui lòng nhập thông tin tài khoản của bạn để tiếp tục",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = MauChuPhu,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, ScalePx(18))
            };
            tlpMain.Controls.Add(lblTieuDe);
            tlpMain.Controls.Add(lblPhuDe);

            // 2. Trường Tên đăng nhập (Xếp dọc: Nhãn ở trên, Ô nhập ở dưới)
            Label lblTen = new Label
            {
                Text = "Tên đăng nhập hoặc Email",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = MauChuChinh,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, ScalePx(6))
            };
            txtTenDangNhap = new TextBox
            {
                Name = "txtTenDangNhap",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F),
                MaxLength = 100,
                PlaceholderText = "Nhập tên đăng nhập hoặc email",
                Margin = new Padding(0, 0, 0, ScalePx(14)),
                TabIndex = 0
            };
            tlpMain.Controls.Add(lblTen);
            tlpMain.Controls.Add(txtTenDangNhap);

            // 3. Trường Mật khẩu
            Label lblMK = new Label
            {
                Text = "Mật khẩu",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = MauChuChinh,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, ScalePx(6))
            };
            txtMatKhau = new TextBox
            {
                Name = "txtMatKhau",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10.5F),
                UseSystemPasswordChar = true,
                PlaceholderText = "Nhập mật khẩu của bạn",
                Margin = new Padding(0, 0, 0, ScalePx(8)),
                TabIndex = 1
            };
            tlpMain.Controls.Add(lblMK);
            tlpMain.Controls.Add(txtMatKhau);

            // Tuỳ chọn hiện mật khẩu
            CheckBox chkHienMatKhau = new CheckBox
            {
                Text = "Hiển thị mật khẩu",
                Font = new Font("Segoe UI", 9F),
                ForeColor = MauChuPhu,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, ScalePx(6)),
                TabIndex = 2
            };
            chkHienMatKhau.CheckedChanged += (s, e) =>
            {
                txtMatKhau.UseSystemPasswordChar = !chkHienMatKhau.Checked;
            };
            tlpMain.Controls.Add(chkHienMatKhau);

            // 4. Nhãn thông báo lỗi luôn hiện diện với chiều cao cố định (không làm giật Form)
            lblThongBaoLoi = new Label
            {
                Name = "lblThongBaoLoi",
                Text = "",
                ForeColor = MauLoi,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Dock = DockStyle.Fill,
                AutoSize = true,
                MinimumSize = new Size(0, ScalePx(26)),
                Margin = new Padding(0, 0, 0, ScalePx(6))
            };
            tlpMain.Controls.Add(lblThongBaoLoi);

            // 5. Cụm 2 nút bấm chia đều 2 cột theo tỷ lệ phần trăm
            TableLayoutPanel tlpButtons = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Margin = new Padding(0, 0, 0, ScalePx(16))
            };
            tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            tlpButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));

            btnDangNhap = new Button
            {
                Name = "btnDangNhap",
                Text = "Đăng nhập",
                Dock = DockStyle.Fill,
                MinimumSize = new Size(0, ScalePx(40)),
                AutoSize = true,
                BackColor = MauChuDao,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, ScalePx(8), 0),
                TabIndex = 3
            };
            btnDangNhap.FlatAppearance.BorderSize = 0;
            btnDangNhap.Click += BtnDangNhap_Click;

            btnThoat = new Button
            {
                Name = "btnThoat",
                Text = "Thoát",
                Dock = DockStyle.Fill,
                MinimumSize = new Size(0, ScalePx(40)),
                AutoSize = true,
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = MauChuChinh,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0),
                TabIndex = 4
            };
            btnThoat.FlatAppearance.BorderSize = 0;
            btnThoat.Click += (s, e) => Application.Exit();

            tlpButtons.Controls.Add(btnDangNhap, 0, 0);
            tlpButtons.Controls.Add(btnThoat, 1, 0);
            tlpMain.Controls.Add(tlpButtons);

            // 6. Liên kết chuyển sang Form Đăng ký căn giữa
            lnkDangKy = new LinkLabel
            {
                Name = "lnkDangKy",
                Text = "Chưa có tài khoản? Đăng ký ngay",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                LinkColor = MauChuDao,
                ActiveLinkColor = Color.FromArgb(29, 78, 216),
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Margin = new Padding(0, ScalePx(4), 0, 0),
                TabIndex = 5
            };
            lnkDangKy.LinkClicked += LnkDangKy_LinkClicked;
            tlpMain.Controls.Add(lnkDangKy);

            pnlCard.Controls.Add(tlpMain);
            this.Controls.Add(pnlCard);

            this.AcceptButton = btnDangNhap;
            this.CancelButton = btnThoat;
        }

        private async void BtnDangNhap_Click(object? sender, EventArgs e)
        {
            lblThongBaoLoi.Text = "";

            string dinhDanh = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text;

            // 1. Kiểm tra rỗng trên Client trước khi gọi xuống CSDL
            if (string.IsNullOrEmpty(dinhDanh))
            {
                lblThongBaoLoi.Text = "Vui lòng nhập tên đăng nhập hoặc email.";
                txtTenDangNhap.Focus();
                return;
            }

            // Nếu người dùng nhập định danh có chứa '@' (đăng nhập bằng Email) thì kiểm tra khuôn dạng Email trước
            if (dinhDanh.Contains('@') && !Regex.IsMatch(dinhDanh, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblThongBaoLoi.Text = "Địa chỉ email không hợp lệ (vd: ten@mien.com).";
                txtTenDangNhap.Focus();
                return;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                lblThongBaoLoi.Text = "Vui lòng nhập mật khẩu.";
                txtMatKhau.Focus();
                return;
            }

            // 2. Khoá nút và đổi nhãn trong lúc chờ CSDL phản hồi (Bước 6)
            btnDangNhap.Enabled = false;
            btnDangNhap.Text = "Đang xử lý...";

            try
            {
                NguoiDung? nguoiDung = await _repo.DangNhapAsync(dinhDanh, matKhau);

                if (nguoiDung == null)
                {
                    // Thông báo chung cho cả trường hợp sai tên lẫn sai mật khẩu + xoá trắng ô mật khẩu
                    lblThongBaoLoi.Text = "Tên đăng nhập hoặc mật khẩu không đúng";
                    txtTenDangNhap.Clear();
                    txtMatKhau.Clear();
                    txtTenDangNhap.Focus();
                    return;
                }

                // Đăng nhập thành công -> Mở Form Chính
                txtMatKhau.Clear();
                lblThongBaoLoi.Text = "";

                using var formChinh = new FormChinh(nguoiDung);
                this.Hide();
                formChinh.ShowDialog();
                this.Show();
                txtTenDangNhap.Focus();
            }
            catch (SqlException)
            {
                // Bắt lỗi khi dịch vụ SQL Server bị dừng hoặc lỗi mạng (Tiêu chí 6 & Bước 7 - STT 8)
                lblThongBaoLoi.Text = "Không thể kết nối đến cơ sở dữ liệu. Vui lòng thử lại sau.";
            }
            catch (Exception)
            {
                lblThongBaoLoi.Text = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại.";
            }
            finally
            {
                // Luôn mở khoá lại nút sau khi xử lý xong
                btnDangNhap.Enabled = true;
                btnDangNhap.Text = "Đăng nhập";
            }
        }

        private void LnkDangKy_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            lblThongBaoLoi.Text = "";
            using var formDangKy = new FormDangKy();
            this.Hide();
            if (formDangKy.ShowDialog() == DialogResult.OK)
            {
                // Điền sẵn tên đăng nhập vừa tạo thành công để tiện đăng nhập
                txtTenDangNhap.Text = formDangKy.TenDangNhapVuaTao;
                txtMatKhau.Clear();
            }
            this.Show();
            txtMatKhau.Focus();
        }
    }
}
