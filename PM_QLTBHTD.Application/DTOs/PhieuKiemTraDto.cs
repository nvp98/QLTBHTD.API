namespace PM_QLTBHTD.Application.DTOs
{
    public class PhieuKiemTraDto
    {
        public int ID_Phieu { get; set; }
        public int ID_ThietBi { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public DateTime NgayKiemTra { get; set; }
        public string? NguoiKiemTra { get; set; }
        public decimal? TongDiem_Soqt { get; set; }
        public string? CapDoCanhBao { get; set; }
        public string? GhiChuChung { get; set; }
    }

    public class CreatePhieuKiemTraDto
    {
        public int ID_ThietBi { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public string? NguoiKiemTra { get; set; }
        public string? GhiChuChung { get; set; }
        public List<CreateChiTietKiemTraDto> ChiTiets { get; set; } = new();
    }

    public class UpdatePhieuKiemTraDto
    {
        public int ID_ThietBi { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public string? NguoiKiemTra { get; set; }
        public decimal? TongDiem_Soqt { get; set; }
        public string? CapDoCanhBao { get; set; }
        public string? GhiChuChung { get; set; }
    }

    public class PhieuKiemTraDetailDto : PhieuKiemTraDto
    {
        public List<ChiTietKiemTraDto> ChiTiets { get; set; } = new();
    }
}
