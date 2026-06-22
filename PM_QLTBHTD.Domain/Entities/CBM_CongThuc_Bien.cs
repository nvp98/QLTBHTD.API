using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Ánh xạ biến trong BieuThuc của CBM_CongThucTongHop tới nguồn dữ liệu thực.
    /// </summary>
    public class CBM_CongThuc_Bien
    {
        [Key]
        public int ID_Bien { get; set; }

        /// <summary>FK logic tới CBM_CongThucTongHop.</summary>
        public int ID_CongThuc { get; set; }

        /// <summary>Tên biến trong BieuThuc (phân biệt hoa/thường).</summary>
        public string MaBien { get; set; } = string.Empty;

        /// <summary>
        /// 'CHITIEU'  → lấy Diem_Si_DatDuoc từ ChiTietKiemTra theo ID_ChiTieuNguon.
        /// 'NHOM_CON' → gọi đệ quy TinhDiemNhomAsync theo ID_NhomCon.
        /// 'HANGSO'   → dùng GiaTriHangSo cố định.
        /// </summary>
        public string NguonBien { get; set; } = "HANGSO";

        /// <summary>ID_ChiTieu nguồn khi NguonBien='CHITIEU'.</summary>
        public int? ID_ChiTieuNguon { get; set; }

        /// <summary>ID_NhomChiTieu con khi NguonBien='NHOM_CON'.</summary>
        public int? ID_NhomCon { get; set; }

        /// <summary>Giá trị cố định khi NguonBien='HANGSO' (ví dụ hệ số 0.6, trọng số 10).</summary>
        public decimal? GiaTriHangSo { get; set; }

        public string? MoTa { get; set; }
    }
}
