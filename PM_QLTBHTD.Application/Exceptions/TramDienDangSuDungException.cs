namespace PM_QLTBHTD.Application.Exceptions
{
    public class TramDienDangSuDungException : Exception
    {
        public int IdTram { get; }
        public int SoThietBiDangDung { get; }

        public TramDienDangSuDungException(int idTram, int soThietBiDangDung)
            : base($"Không thể xóa — trạm điện này đang có {soThietBiDangDung} thiết bị. " +
                   "Xóa hoặc chuyển các thiết bị đó sang trạm khác trước khi xóa.")
        {
            IdTram = idTram;
            SoThietBiDangDung = soThietBiDangDung;
        }
    }
}
