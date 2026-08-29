using System.ComponentModel.DataAnnotations;

namespace PM_QLTBHTD.Domain.Entities
{
    /// <summary>
    /// Thông số kỹ thuật cố định (nhãn máy) của 1 thiết bị — dạng key-value, không thêm cột riêng
    /// cho từng thông số (như CBM_ThietBi.TaiDinhMuc đã làm trước đây) để không phải sửa schema mỗi
    /// khi phát sinh thông số mới. VD: Ir (dòng định mức động cơ OLTC), điện áp định mức, v.v.
    /// Dùng làm nguồn giá trị 'THIETBI_THONGSO' cho CBM_ChiTieu_Input — nhập 1 lần khi khai báo
    /// thiết bị, không cần nhập lại mỗi lần lập phiếu kiểm tra.
    /// </summary>
    public class CBM_ThietBi_ThongSo
    {
        [Key]
        public int ID_ThietBi_ThongSo { get; set; }

        public int ID_ThietBi { get; set; }

        public int ID_ThongSo { get; set; }

        public decimal GiaTri { get; set; }

        public string? GhiChu { get; set; }
    }
}
