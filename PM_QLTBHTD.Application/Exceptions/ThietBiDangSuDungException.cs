namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThietBiDangSuDungException : Exception
    {
        public int IdThietBi { get; }
        public int SoPhieuDangDung { get; }
        public int SoLichBaoTriDangDung { get; }

        public ThietBiDangSuDungException(int idThietBi, int soPhieuDangDung, int soLichBaoTriDangDung)
            : base($"Không thể xóa — thiết bị này đã có {soPhieuDangDung} phiếu kiểm tra và {soLichBaoTriDangDung} " +
                   "lịch bảo trì. Xóa dữ liệu liên quan trước khi xóa thiết bị.")
        {
            IdThietBi = idThietBi;
            SoPhieuDangDung = soPhieuDangDung;
            SoLichBaoTriDangDung = soLichBaoTriDangDung;
        }
    }
}
