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
    }
}
