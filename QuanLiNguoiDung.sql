-- Tạo cơ sở dữ liệu QuanLyNguoiDung
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'QuanLyNguoiDung')
BEGIN
    CREATE DATABASE QuanLyNguoiDung;
END
GO

USE QuanLyNguoiDung;
GO

-- Tạo bảng Users theo đúng chuẩn Mục III.4
IF OBJECT_ID('Users', 'U') IS NOT NULL
    DROP TABLE Users;
GO

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
GO

-- Kiểm tra bảng vừa tạo
SELECT * FROM Users;