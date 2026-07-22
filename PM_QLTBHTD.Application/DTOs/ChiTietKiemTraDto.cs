namespace PM_QLTBHTD.Application.DTOs
{
    /// <summary>1 giá trị biến đầu vào thô (vd T_tren=48) đã lưu cho 1 chỉ tiêu trong 1 phiếu cụ thể.</summary>
    public class ChiTietInputValueDto
    {
        public string MaInput { get; set; } = string.Empty;
        public string? TenInput { get; set; }
        public decimal GiaTriSo { get; set; }
    }

    public class ChiTietKiemTraDto
    {
        public int ID_ChiTiet { get; set; }
        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }
        public string TenChiTieu { get; set; } = string.Empty;
        public decimal? GiaTriNhap_So { get; set; }
        public string? GiaTriNhap_Chu { get; set; }
        public decimal? Diem_Si_DatDuoc { get; set; }
        public string? GhiChu { get; set; }
        /// <summary>Giá trị Input thô (T_tren, T_duoi...) khi chỉ tiêu dùng Rule/Formula nhiều biến —
        /// NULL/rỗng nếu chỉ tiêu chỉ nhập 1 giá trị đơn (đã có ở GiaTriNhap_So).</summary>
        public List<ChiTietInputValueDto>? DanhSachInput { get; set; }
    }

    /// <summary>1 lần đo trong quá khứ của cùng 1 chỉ tiêu, cùng 1 thiết bị — dùng cho modal "Lịch sử đo".</summary>
    public class LichSuChiTieuDto
    {
        public int ID_Phieu { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public decimal? GiaTriNhap_So { get; set; }
        public string? GiaTriNhap_Chu { get; set; }
        public decimal? Diem_Si_DatDuoc { get; set; }
        public List<ChiTietInputValueDto>? DanhSachInput { get; set; }
    }

    public class CreateChiTietKiemTraDto
    {
        public int ID_ChiTieu { get; set; }
        public decimal? GiaTriNhap_So { get; set; }
        public string? GiaTriNhap_Chu { get; set; }
        public string? GhiChu { get; set; }
    }

    public class UpdateChiTietKiemTraDto
    {
        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }
        public decimal? GiaTriNhap_So { get; set; }
        public string? GiaTriNhap_Chu { get; set; }
        public decimal? Diem_Si_DatDuoc { get; set; }
        public string? GhiChu { get; set; }
    }
}
