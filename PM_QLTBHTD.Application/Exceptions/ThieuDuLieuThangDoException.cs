namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuDuLieuThangDoException : Exception
    {
        public int IdChiTieu { get; }
        public int IdPhieu { get; }

        public ThieuDuLieuThangDoException(int idChiTieu, int idPhieu)
            : base($"Chỉ tiêu ID={idChiTieu} có LoaiTinhDiem='LF' nhưng phiếu ID={idPhieu} " +
                   "chưa gửi DanhSachThang (danh sách giá trị đo theo tháng).")
        {
            IdChiTieu = idChiTieu;
            IdPhieu = idPhieu;
        }
    }
}
