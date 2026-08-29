using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class CBM_ChiTieu_Input
    {
        [Key]
        public int ID_Input { get; set; }
        public int ID_ChiTieu { get; set; }
        public string MaInput { get; set; }
        public string TenInput { get; set; }
        /// <summary>'MANUAL' (mặc định, người dùng nhập tay) | 'CHITIEU_CUNG_PHIEU' (tự lấy
        /// GiaTriNhap_So của 1 chỉ tiêu khác trong CÙNG phiếu, không cần nhập tay — vd TDCG lấy
        /// từ 6 chỉ tiêu khí thành phần) | 'THIETBI_THONGSO' (tự lấy thông số kỹ thuật cố định của
        /// thiết bị, vd Ir — nhập 1 lần khi khai báo thiết bị, không cần nhập lại mỗi phiếu).</summary>
        public string? NguonGiaTri { get; set; }
        /// <summary>Chỉ tiêu nguồn khi NguonGiaTri='CHITIEU_CUNG_PHIEU'.</summary>
        public int? ID_ChiTieuNguon { get; set; }
        /// <summary>Mã thông số (khớp CBM_ThietBi_ThongSo.MaThongSo) khi NguonGiaTri='THIETBI_THONGSO'.</summary>
        public string? MaThongSoThietBi { get; set; }
    }
}
