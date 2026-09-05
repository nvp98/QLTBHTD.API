namespace PM_QLTBHTD.Application.DTOs
{
    public class ThongKeTongHopDto
    {
        public int TongThietBi { get; set; }
        public int ThietBiTot { get; set; }
        public int ThietBiBinhThuong { get; set; }
        public int ThietBiChuY { get; set; }
        public int ThietBiCanhBao { get; set; }
        public int ThietBiNguyHiem { get; set; }
        public int ThietBiChuaKiemTra { get; set; }
        public int TongPhieuThangNay { get; set; }
        public double? DiemTrungBinh { get; set; }
    }

    public class LichSuCSSKDto
    {
        public int ID_Phieu { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public decimal? TongDiem_Soqt { get; set; }
        public string? CapDoCanhBao { get; set; }
        public string? NguoiKiemTra { get; set; }
    }

    public class BaoCaoTramItemDto
    {
        public int IDTram { get; set; }
        public string TenTram { get; set; } = string.Empty;
        public string? DiaDiem { get; set; }
        public int TongThietBi { get; set; }
        public int DaKiemTra { get; set; }
        public double? DiemTrungBinh { get; set; }
        public int TotCount { get; set; }
        public int BinhThuongCount { get; set; }
        public int ChuYCount { get; set; }
        public int CanhBaoCount { get; set; }
        public int NguHiemCount { get; set; }
    }

    /// <summary>CSSK trung bình + phân bố hạng theo TỪNG LOẠI THIẾT BỊ — vì các loại thiết bị dùng
    /// bộ chỉ tiêu/công thức khác nhau, KHÔNG nên gộp chung 1 số trung bình toàn hệ thống.</summary>
    public class TongHopTheoLoaiDto
    {
        public int ID_LoaiTB { get; set; }
        public string TenLoaiTB { get; set; } = string.Empty;
        public string? KyHieu { get; set; }
        public int TongThietBi { get; set; }
        public int DaKiemTra { get; set; }
        public double? DiemTrungBinh { get; set; }
        public int TotCount { get; set; }
        public int BinhThuongCount { get; set; }
        public int ChuYCount { get; set; }
        public int CanhBaoCount { get; set; }
        public int NguHiemCount { get; set; }
    }

    public class CanhBaoThietBiDto
    {
        public int ID_ThietBi { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public string TenTram { get; set; } = string.Empty;
        public string? KyHieu { get; set; }
        public int ID_Phieu { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public decimal? TongDiem_Soqt { get; set; }
        public string CapDoCanhBao { get; set; } = string.Empty;

        /// <summary>Điểm dùng để phân loại/sắp xếp/tô màu. Bằng TongDiem_Soqt khi NguonDiem='CSSK';
        /// bằng Sᵢ của chỉ tiêu thấp nhất khi NguonDiem='CHI_TIEU' hoặc 'CHI_TIEU_RIENG'.</summary>
        public decimal DiemHienThi { get; set; }

        /// <summary>'CSSK' = DiemHienThi lấy từ TongDiem_Soqt (điểm tổng cũng đang ở mức cảnh báo).
        /// 'CHI_TIEU' = lấy từ Sᵢ thấp nhất 1 chỉ tiêu (thiết bị không có CSSK tổng, vd DCL/TU/TI/CS).
        /// 'CHI_TIEU_RIENG' = CSSK TỔNG vẫn ≥6 (Khá/Tốt) nhưng có 1 chỉ tiêu riêng lẻ rơi vào mức
        /// Cảnh báo/Nguy hiểm (Sᵢ &lt; 4) bị trọng số nhỏ "trung hòa" mất trong điểm tổng — vẫn cần
        /// cảnh báo theo đúng chỉ tiêu đó thay vì bỏ sót chỉ vì điểm tổng qua ngưỡng. TongDiem_Soqt
        /// vẫn giữ giá trị thật (tốt) để FE hiển thị kèm, tránh hiểu nhầm cả thiết bị đang kém.</summary>
        public string NguonDiem { get; set; } = "CSSK";

        /// <summary>Tên chỉ tiêu có Sᵢ thấp nhất — có giá trị khi NguonDiem='CHI_TIEU' hoặc 'CHI_TIEU_RIENG'.</summary>
        public string? TenChiTieuThapNhat { get; set; }

        /// <summary>Khuyến cáo hành động (snapshot) của chỉ tiêu có Sᵢ thấp nhất trong phiếu — giúp
        /// người xem biết ngay cần làm gì mà không phải mở chi tiết phiếu.</summary>
        public string? KhuyenCaoHanhDong { get; set; }
    }

    /// <summary>Một điểm dữ liệu xu hướng CSSK trung bình toàn hệ thống theo tháng.</summary>
    public class XuHuongThangDto
    {
        /// <summary>Định dạng "yyyy-MM", ví dụ "2026-03".</summary>
        public string Thang { get; set; } = string.Empty;
        public double? DiemTrungBinh { get; set; }
        public int SoPhieu { get; set; }
    }
}
