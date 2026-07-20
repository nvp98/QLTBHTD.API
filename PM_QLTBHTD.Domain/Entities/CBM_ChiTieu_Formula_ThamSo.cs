using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Ánh xạ 1 tham số/biến dùng trong CBM_ChiTieu_Formula.BieuThuc (hoặc tham số của TenFunction)
    /// tới nguồn dữ liệu thật.
    /// </summary>
    public class CBM_ChiTieu_Formula_ThamSo
    {
        [Key]
        public int ID_ThamSo { get; set; }
        public int ID_Formula { get; set; }

        /// <summary>Tên biến trong BieuThuc, hoặc tên tham số theo signature của Function.</summary>
        public string MaThamSo { get; set; } = string.Empty;

        /// <summary>
        /// 'INPUT'             → giá trị đo, khớp CBM_ChiTieu_Input.MaInput.
        /// 'FORMULA_KETQUA'    → kết quả của 1 Formula khác cùng Chỉ tiêu (ThuTu nhỏ hơn).
        /// 'HANGSO'            → giá trị cố định (GiaTriHangSo).
        /// 'CHITIEU_SI'        → Si đã chốt của 1 Chỉ tiêu khác.
        /// 'THIETBI_THUOCTINH' → thuộc tính của CBM_ThietBi (vd 'TaiDinhMuc').
        /// </summary>
        public string NguonGiaTri { get; set; } = "HANGSO";

        public string? MaInput { get; set; }
        public int? ID_FormulaNguon { get; set; }
        public int? ID_ChiTieuNguon { get; set; }
        public string? TenThuocTinhTB { get; set; }
        public decimal? GiaTriHangSo { get; set; }
    }
}
