using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    public class ChiTietKiemTra
    {
        [Key]
        public int ID_ChiTiet { get; set; }
        public int IDPhieu { get; set; }
        public int ID_ChiTieu { get; set; }

        public decimal? GiaTriNhap_So { get; set; }
        public string? GiaTriNhap_Chu { get; set; }

        public decimal? Diem_Si_DatDuoc { get; set; }

        /// <summary>
        /// Snapshot hành động khuyến cáo tại đúng thời điểm chấm điểm (copy từ CBM_Nguong/CBM_ChiTieu_Rule
        /// của dòng đã khớp) — giữ nguyên dù sau này sửa lại nội dung khuyến cáo gốc, để tra cứu lịch sử đúng.
        /// </summary>
        public string? HanhDongKhuyenCao { get; set; }

        public string? GhiChu { get; set; }

    }
}
