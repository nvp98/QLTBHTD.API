namespace PM_QLTBHTD.Application.DTOs
{
    public class LichSuChiTieuColDto
    {
        public int ID_ChiTieu { get; set; }
        public string TenChiTieu { get; set; } = string.Empty;
    }

    public class LichSuGiaTriDto
    {
        public decimal? GiaTri { get; set; }
        public decimal? Si { get; set; }
    }

    public class LichSuHangDto
    {
        public int ID_Phieu { get; set; }
        public DateTime NgayKiemTra { get; set; }
        public string? SoPhieu { get; set; }
        /// <summary>Điểm của CHÍNH nhóm chỉ tiêu đang xem, tại thời điểm phiếu này (nếu đã tính) —
        /// lấy từ CBM_KetQuaNhom, KHÔNG phải CSSK tổng toàn thiết bị.</summary>
        public decimal? DiemNhom { get; set; }
        /// <summary>Key = ID_ChiTieu.</summary>
        public Dictionary<int, LichSuGiaTriDto> GiaTriTheoChiTieu { get; set; } = new();
    }

    public class LichSuNhomDto
    {
        public int ID_ThietBi { get; set; }
        public string TenThietBi { get; set; } = string.Empty;
        public int ID_NhomChiTieu { get; set; }
        public string TenNhom { get; set; } = string.Empty;
        public List<LichSuChiTieuColDto> ChiTieus { get; set; } = new();
        /// <summary>Sắp xếp theo NgayKiemTra TĂNG DẦN (cũ → mới) — thuận cho vẽ xu hướng.</summary>
        public List<LichSuHangDto> Hang { get; set; } = new();
    }
}
