namespace PM_QLTBHTD.Application.Exceptions
{
    public class ChiTieuDangSuDungException : Exception
    {
        public int IdChiTieu { get; }
        public int SoPhieuDangDung { get; }
        public int SoCongThucThamChieu { get; }

        public ChiTieuDangSuDungException(int idChiTieu, int soPhieuDangDung, int soCongThucThamChieu)
            : base($"Không thể xóa — chỉ tiêu này đã có {soPhieuDangDung} kết quả kiểm tra và/hoặc đang được " +
                   $"{soCongThucThamChieu} công thức tổng hợp khác tham chiếu. Xóa dữ liệu/tham chiếu liên quan trước.")
        {
            IdChiTieu = idChiTieu;
            SoPhieuDangDung = soPhieuDangDung;
            SoCongThucThamChieu = soCongThucThamChieu;
        }
    }
}
