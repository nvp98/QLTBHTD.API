namespace PM_QLTBHTD.Application.DTOs
{
    public class ChiTieuPhanLoaiNguongDto
    {
        public int ID_PhanLoai { get; set; }
        public int ID_ChiTieu { get; set; }
        public string MaMuc { get; set; } = string.Empty;
        public decimal? GiaTriTu { get; set; }
        public decimal? GiaTriDen { get; set; }
        public bool GiaTriTu_BaoGom { get; set; }
        public bool GiaTriDen_BaoGom { get; set; }
        public decimal TrongSo { get; set; }
        public int ThuTu { get; set; }
    }

    public class CreateChiTieuPhanLoaiNguongDto
    {
        public int ID_ChiTieu { get; set; }
        public string MaMuc { get; set; } = string.Empty;
        public decimal? GiaTriTu { get; set; }
        public decimal? GiaTriDen { get; set; }
        public bool GiaTriTu_BaoGom { get; set; }
        public bool GiaTriDen_BaoGom { get; set; }
        public decimal TrongSo { get; set; }
        public int ThuTu { get; set; }
    }

    public class UpdateChiTieuPhanLoaiNguongDto
    {
        public string MaMuc { get; set; } = string.Empty;
        public decimal? GiaTriTu { get; set; }
        public decimal? GiaTriDen { get; set; }
        public bool GiaTriTu_BaoGom { get; set; }
        public bool GiaTriDen_BaoGom { get; set; }
        public decimal TrongSo { get; set; }
        public int ThuTu { get; set; }
    }

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
