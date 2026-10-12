-- Bước 1: Tạo cơ sở dữ liệu
CREATE DATABASE TiemTapHoa;
GO

-- Sử dụng cơ sở dữ liệu vừa tạo
USE TiemTapHoa;
GO

-- Bước 2: Tạo bảng HangHoa
CREATE TABLE HangHoa (
    MaHang INT IDENTITY(1,1) PRIMARY KEY,
    TenHang NVARCHAR(100) NOT NULL,
    LoaiHang NVARCHAR(50),
    DonVi NVARCHAR(20),
    GiaBan DECIMAL(18,0) CHECK (GiaBan > 0),
    SoLuongTon INT CHECK (SoLuongTon >= 0),
    NgayNhap DATETIME,
    ConKinhDoanh BIT -- 1 = còn bán, 0 = ngừng bán
);
GO

-- Bước 3: Chèn 6 dòng dữ liệu mẫu
INSERT INTO HangHoa (TenHang, LoaiHang, DonVi, GiaBan, SoLuongTon, NgayNhap, ConKinhDoanh) VALUES
(N'Nước suối Aquafina 500ml', N'Đồ uống', N'Chai', 6000, 120, '2026-10-02 00:00:00', 1),
(N'Mì Hảo Hảo tôm chua cay', N'Thực phẩm khô', N'Gói', 4500, 200, '2026-10-02 00:00:00', 1),
(N'Dầu ăn Tường An 1L', N'Gia vị', N'Chai', 48000, 35, '2026-10-05 00:00:00', 1),
(N'Bột giặt OMO 800g', N'Đồ gia dụng', N'Túi', 52000, 40, '2026-10-05 00:00:00', 1),
(N'Sữa tươi Vinamilk 1L', N'Đồ uống', N'Hộp', 32000, 60, '2026-10-08 00:00:00', 1),
(N'Nước mắm Nam Ngữ 500ml', N'Gia vị', N'Chai', 38000, 25, '2026-10-01 00:00:00', 0);
GO

-- Kiểm tra lại dữ liệu trong bảng
SELECT * FROM HangHoa;