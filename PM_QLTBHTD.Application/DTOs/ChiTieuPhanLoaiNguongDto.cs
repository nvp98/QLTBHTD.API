namespace PM_QLTBHTD.Application.DTOs
{
    /// <summary>
    /// Định nghĩa 1 mức phân loại (N0..N4) cho chỉ tiêu kiểu LoaiTinhDiem='LF'.
    /// </summary>
    public class ChiTieuPhanLoaiNguongDto
    {
        public int ID_PhanLoai { get; set; }
        public int ID_ChiTieu { get; set; }
        public string MaMuc { get; set; } = string.Empty;
        public decimal? GiaTriTu { get; set; }
        public decimal? GiaTriDen { get; set; }
        public bool GiaTriTu_BaoGom { get; set; }
        public bool GiaTriDen_BaoGom { get; set; }
        /// <summary>Trọng số áp dụng cho tháng rơi vào mức này, dùng để tính LF = ΣTrongSo / SoThang.</summary>
        public decimal TrongSo { get; set; }
        public int ThuTu { get; set; }
    }

    public class CreateChiTieuPhanLoaiNguongDto
    {
        public int ID_ChiTieu { get; set; }
        public string MaMuc { get; set; } = string.Empty;
        public decimal? GiaTriTu { get; set; }
        public decimal? GiaTriDen { get; set; }
        public bool GiaTriTu_BaoGom { get; set; } = true;
        public bool GiaTriDen_BaoGom { get; set; } = false;
        public decimal TrongSo { get; set; }
        public int ThuTu { get; set; }
    }

    public class UpdateChiTieuPhanLoaiNguongDto
    {
        public string MaMuc { get; set; } = string.Empty;
        public decimal? GiaTriTu { get; set; }
        public decimal? GiaTriDen { get; set; }
        public bool GiaTriTu_BaoGom { get; set; } = true;
        public bool GiaTriDen_BaoGom { get; set; } = false;
        public decimal TrongSo { get; set; }
        public int ThuTu { get; set; }
    }

    /// <summary>1 giá trị đo theo tháng, gửi lên khi nhập liệu cho chỉ tiêu kiểu LF.</summary>
    public class ThangDoDto
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public decimal GiaTriDo { get; set; }
    }

    /// <summary>Kết quả phân loại đã tính cho 1 tháng — trả về để hiển thị lại sau khi lưu.</summary>
    public class KetQuaPhanLoaiThangDto
    {
        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }
        public int Nam { get; set; }
        public int Thang { get; set; }
        public decimal GiaTriDo { get; set; }
        public string MaMuc { get; set; } = string.Empty;
        public decimal TrongSo { get; set; }
    }
}
