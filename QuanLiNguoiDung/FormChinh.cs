using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace QuanLiNguoiDung
{
    public partial class FormChinh : Form
    {
        private Label lblXinChao = null!;
        private Label lblTenDangNhap = null!;
        private Label lblEmail = null!;
        private Label lblLanDangNhap = null!;
        private Button btnDangXuat = null!;

        private readonly NguoiDung _nguoiDung;

        private readonly Color MauNenForm = Color.FromArgb(241, 245, 249);
        private readonly Color MauTrangCard = Color.White;
        private readonly Color MauChuDao = Color.FromArgb(37, 99, 235);
        private readonly Color MauChuChinh = Color.FromArgb(15, 23, 42);
        private readonly Color MauChuPhu = Color.FromArgb(100, 116, 139);

        public FormChinh(NguoiDung nguoiDung)
        {
            _nguoiDung = nguoiDung;
            KhoiTaoGiaoDien();
            HienThiThongTin();
        }

        private int ScalePx(int px) => (int)Math.Round(px * (this.DeviceDpi / 96f));
        private void KhoiTaoGiaoDien()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.Text = "Hệ thống Quản lý Người dùng - Trang chính";
            this.BackColor = MauNenForm;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            this.Padding = new Padding(ScalePx(28));

            Panel pnlCard = new Panel
            {
                BackColor = MauTrangCard,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0),
                MinimumSize = new Size(ScalePx(400), 0)
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
                Padding = new Padding(ScalePx(28), ScalePx(24), ScalePx(28), ScalePx(24)),
                MinimumSize = new Size(ScalePx(400), 0)
            };
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            // Nhãn trạng thái phiên đăng nhập
            Label lblTrangThai = new Label
            {
                Text = "● ĐANG HOẠT ĐỘNG",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 163, 74), // Xanh lá trạng thái Online
                AutoSize = true,
                Margin = new Padding(0, ScalePx(2), 0, ScalePx(4))
            };
            tlpMain.Controls.Add(lblTrangThai);

            // Lời chào người dùng
            lblXinChao = new Label
            {
                Name = "lblXinChao",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = MauChuChinh,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, ScalePx(16))
            };
            tlpMain.Controls.Add(lblXinChao);

            // Khung thông tin chi tiết nền xám nhạt hiện đại
            TableLayoutPanel tlpInfoBox = new TableLayoutPanel
            {
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(ScalePx(16), ScalePx(14), ScalePx(16), ScalePx(14)),
                Margin = new Padding(0, 0, 0, ScalePx(20))
            };
            tlpInfoBox.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            lblTenDangNhap = new Label
            {
                Name = "lblTenDangNhap",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = MauChuChinh,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, ScalePx(10))
            };

            lblEmail = new Label
            {
                Name = "lblEmail",
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                ForeColor = MauChuChinh,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, ScalePx(10))
            };

            Label lblTieuDeLanDangNhap = new Label
            {
                Text = "Lần đăng nhập gần nhất:",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = MauChuPhu,
                AutoSize = true,
                Margin = new Padding(0, ScalePx(4), 0, ScalePx(2))
            };

            lblLanDangNhap = new Label
            {
                Name = "lblLanDangNhap",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = MauChuDao,
                AutoSize = true,
                Margin = new Padding(0)
            };

            tlpInfoBox.Controls.Add(lblTenDangNhap);
            tlpInfoBox.Controls.Add(lblEmail);
            tlpInfoBox.Controls.Add(lblTieuDeLanDangNhap);
            tlpInfoBox.Controls.Add(lblLanDangNhap);
            tlpMain.Controls.Add(tlpInfoBox);

            // Nút Đăng xuất
            btnDangXuat = new Button
            {
                Name = "btnDangXuat",
                Text = "Đăng xuất tài khoản",
                Dock = DockStyle.Fill,
                MinimumSize = new Size(0, ScalePx(40)),
                AutoSize = true,
                BackColor = Color.FromArgb(254, 226, 226),
                ForeColor = Color.FromArgb(185, 28, 28),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0),
                TabIndex = 0
            };
            btnDangXuat.FlatAppearance.BorderSize = 0;
            btnDangXuat.Click += (s, e) => this.Close();

            tlpMain.Controls.Add(btnDangXuat);

            pnlCard.Controls.Add(tlpMain);
            this.Controls.Add(pnlCard);

            this.CancelButton = btnDangXuat;
        }

        private void HienThiThongTin()
        {
            lblXinChao.Text = $"Xin chào, {_nguoiDung.HoTen}!";
            lblTenDangNhap.Text = $"👤 Tên đăng nhập:   {_nguoiDung.TenDangNhap}";
            lblEmail.Text = $"✉️ Địa chỉ Email:     {_nguoiDung.Email}";
            lblLanDangNhap.Text = _nguoiDung.LanDangNhapCuoi.HasValue
                ? $"🕒 {_nguoiDung.LanDangNhapCuoi.Value:dd/MM/yyyy HH:mm:ss}"
                : "🕒 Lần đầu đăng nhập";
        }
    }
}
