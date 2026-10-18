using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace QuanLiNguoiDung
{
    public class NguoiDungRepository
    {
        // Chuỗi kết nối chuẩn theo Bước 4 (Đổi localhost\SQLEXPRESS thành localhost nếu dùng bản Developer/Default)
        private static readonly string[] DanhSachServer = new[]
        {
            @"localhost",
            @"localhost\SQLEXPRESS",
            @".\SQLEXPRESS",
            @"(localdb)\MSSQLLocalDB"
        };

        private static string? _serverHienTai = null;
        private static bool _daKhoiTaoCSDL = false;

        private static string TaoChuoiKetNoi(string server, string database)
        {
            return $@"Server={server};Database={database};Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=3;";
        }
        private async Task<string> LayChuoiKetNoiHoatDongAsync()
        {
            if (_serverHienTai == null)
            {
                SqlException? loiCuoi = null;
                foreach (string server in DanhSachServer)
                {
                    try
                    {
                        using var connTest = new SqlConnection(TaoChuoiKetNoi(server, "master"));
                        await connTest.OpenAsync();
                        _serverHienTai = server;
                        Debug.WriteLine($"[SQL] Đã kết nối thành công tới: {server}");
                        break;
                    }
                    catch (SqlException ex)
                    {
                        loiCuoi = ex;
                    }
                }

                if (_serverHienTai == null)
                {
                    throw loiCuoi ?? new Exception("Không tìm thấy dịch vụ SQL Server nào đang chạy.");
                }
            }

            if (!_daKhoiTaoCSDL)
            {
                using (var connMaster = new SqlConnection(TaoChuoiKetNoi(_serverHienTai, "master")))
                {
                    await connMaster.OpenAsync();
                    const string sqlCreateDb = @"
                        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'QuanLyNguoiDung')
                        BEGIN
                            CREATE DATABASE QuanLyNguoiDung;
                        END";
                    using var cmdDb = new SqlCommand(sqlCreateDb, connMaster);
                    await cmdDb.ExecuteNonQueryAsync();
                }

                // Tạo bảng Users với cột Email là NOT NULL UNIQUE theo thiết kế mới
                using (var connApp = new SqlConnection(TaoChuoiKetNoi(_serverHienTai, "QuanLyNguoiDung")))
                {
                    await connApp.OpenAsync();
                    const string sqlCreateTable = @"
                        IF OBJECT_ID('Users', 'U') IS NULL
                        BEGIN
                            CREATE TABLE Users (
                                UserId          INT IDENTITY(1,1) PRIMARY KEY,
                                TenDangNhap     NVARCHAR(20)  NOT NULL UNIQUE,
                                MatKhauBam      NVARCHAR(64)  NOT NULL,
                                Salt            NVARCHAR(32)  NOT NULL,
                                HoTen           NVARCHAR(50)  NOT NULL,
                                Email           NVARCHAR(100) NOT NULL UNIQUE,
                                NgayTao         DATETIME2     NOT NULL DEFAULT SYSDATETIME(),
                                LanDangNhapCuoi DATETIME2     NULL
                            );
                        END";
                    using var cmdTable = new SqlCommand(sqlCreateTable, connApp);
                    await cmdTable.ExecuteNonQueryAsync();
                }

                _daKhoiTaoCSDL = true;
            }

            return TaoChuoiKetNoi(_serverHienTai, "QuanLyNguoiDung");
        }

        // 1. Kiểm tra tên đăng nhập đã tồn tại chưa
        public async Task<bool> KiemTraTonTaiTenDangNhapAsync(string tenDangNhap)
        {
            string chuoiKetNoi = await LayChuoiKetNoiHoatDongAsync();
            const string sql = "SELECT COUNT(1) FROM Users WHERE TenDangNhap = @ten";

            using var conn = new SqlConnection(chuoiKetNoi);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;

            await conn.OpenAsync();
            object? result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> KiemTraTonTaiEmailAsync(string email)
        {
            string chuoiKetNoi = await LayChuoiKetNoiHoatDongAsync();
            const string sql = "SELECT COUNT(1) FROM Users WHERE Email = @email";

            using var conn = new SqlConnection(chuoiKetNoi);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email;

            await conn.OpenAsync();
            object? result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        // 2. Thêm tài khoản mới vào bảng Users
        public async Task ThemNguoiDungAsync(string tenDangNhap, string matKhauBam, string salt, string hoTen, string email)
        {
            string chuoiKetNoi = await LayChuoiKetNoiHoatDongAsync();
            const string sql = @"
                INSERT INTO Users (TenDangNhap, MatKhauBam, Salt, HoTen, Email)
                VALUES (@ten, @bam, @salt, @hoTen, @email)";

            using var conn = new SqlConnection(chuoiKetNoi);
            using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@ten", SqlDbType.NVarChar, 20).Value = tenDangNhap;
            cmd.Parameters.Add("@bam", SqlDbType.NVarChar, 64).Value = matKhauBam;
            cmd.Parameters.Add("@salt", SqlDbType.NVarChar, 32).Value = salt;
            cmd.Parameters.Add("@hoTen", SqlDbType.NVarChar, 50).Value = hoTen;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar, 100).Value = email;
                

            await conn.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
        }

        // 3. Xác thực đăng nhập và cập nhật thời gian đăng nhập gần nhất
        public async Task<NguoiDung?> DangNhapAsync(string dinhDanh, string matKhauNhap)
        {
            string chuoiKetNoi = await LayChuoiKetNoiHoatDongAsync();
            const string sqlSelect = @"
                SELECT UserId, TenDangNhap, Salt, MatKhauBam, HoTen, Email, LanDangNhapCuoi
                FROM Users
                WHERE TenDangNhap = @dinhDanh OR Email = @dinhDanh";

            using var conn = new SqlConnection(chuoiKetNoi);
            await conn.OpenAsync();

            int userId;
            string saltLuu, bamLuu, hoTen, email, tenChuan;

            using (var cmd = new SqlCommand(sqlSelect, conn))
            {
                cmd.Parameters.Add("@dinhDanh", SqlDbType.NVarChar, 100).Value = dinhDanh;
                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return null; // Không tồn tại tên đăng nhập
                }

                userId = reader.GetInt32(0);
                tenChuan = reader.GetString(1);
                saltLuu = reader.GetString(2);
                bamLuu = reader.GetString(3);
                hoTen = reader.GetString(4);
                email = reader.GetString(5);
            }

            // Kiểm tra mật khẩu bằng PBKDF2 và FixedTimeEquals
            bool hopLe = MatKhau.KiemTra(matKhauNhap, saltLuu, bamLuu);
            if (!hopLe)
            {
                return null; // Sai mật khẩu
            }

            // Cập nhật thời điểm đăng nhập gần nhất và lấy giá trị vừa cập nhật
            DateTime thoiDiemHienTai = DateTime.Now;
            const string sqlUpdate = @"
                UPDATE Users
                SET LanDangNhapCuoi = @thoiDiem
                WHERE UserId = @id";

            using (var cmdUpdate = new SqlCommand(sqlUpdate, conn))
            {
                cmdUpdate.Parameters.Add("@thoiDiem", SqlDbType.DateTime2).Value = thoiDiemHienTai;
                cmdUpdate.Parameters.Add("@id", SqlDbType.Int).Value = userId;
                await cmdUpdate.ExecuteNonQueryAsync();
            }

            return new NguoiDung
            {
                UserId = userId,
                TenDangNhap = tenChuan,
                HoTen = hoTen,
                Email = email,
                LanDangNhapCuoi = thoiDiemHienTai
            };
        }
    }
}
