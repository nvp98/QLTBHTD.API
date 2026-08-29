namespace PM_QLTBHTD.Application.Exceptions
{
    public class NhomChiTieuDangSuDungException : Exception
    {
        public int IdNhomChiTieu { get; }
        public int SoChiTieu { get; }
        public int SoNhomCon { get; }
        public int SoPhieu { get; }
        public int SoCongThucThamChieu { get; }

        public NhomChiTieuDangSuDungException(int idNhomChiTieu, int soChiTieu, int soNhomCon, int soPhieu, int soCongThucThamChieu)
            : base($"Không thể xóa — nhóm chỉ tiêu này đang có {soChiTieu} chỉ tiêu, {soNhomCon} nhóm con, " +
                   $"{soPhieu} phiếu kiểm tra và/hoặc đang được {soCongThucThamChieu} công thức khác tham chiếu " +
                   "(NHOM_CON). Xóa/di chuyển dữ liệu liên quan trước.")
        {
            IdNhomChiTieu = idNhomChiTieu;
            SoChiTieu = soChiTieu;
            SoNhomCon = soNhomCon;
            SoPhieu = soPhieu;
            SoCongThucThamChieu = soCongThucThamChieu;
        }
    }
}
