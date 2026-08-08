namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuGiaTriL1Exception : Exception
    {
        public int IdChiTieu { get; }

        public ThieuGiaTriL1Exception(int idChiTieu)
            : base($"Chỉ tiêu ID={idChiTieu} có LoaiTinhDiem='TOC_DO_SINH_KHI' nhưng chưa cấu hình " +
                   "Ngưỡng L1 (GiaTri_L1). Vui lòng nhập Ngưỡng L1 trước khi nhập số liệu.")
        {
            IdChiTieu = idChiTieu;
        }
    }
}
