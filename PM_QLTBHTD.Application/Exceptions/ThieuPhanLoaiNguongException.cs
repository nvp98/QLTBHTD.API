namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuPhanLoaiNguongException : Exception
    {
        public int IdChiTieu { get; }

        public ThieuPhanLoaiNguongException(int idChiTieu)
            : base($"Chỉ tiêu ID={idChiTieu} có LoaiTinhDiem='LF' nhưng chưa cấu hình mức phân loại theo tháng " +
                   "(CBM_ChiTieu_PhanLoaiNguong). Vui lòng thêm ít nhất 1 mức (VD: N0..N4) trước khi nhập số liệu.")
        {
            IdChiTieu = idChiTieu;
        }
    }
}
