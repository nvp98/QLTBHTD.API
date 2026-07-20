namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuTaiDinhMucException : Exception
    {
        public int IdThietBi { get; }

        public ThieuTaiDinhMucException(int idThietBi)
            : base($"Thiết bị ID={idThietBi} chưa cấu hình Tải định mức (SB) nên không thể tính tỉ số Si/SB " +
                   "cho chỉ tiêu kiểu LF. Vui lòng nhập Tải định mức ở Quản lý thiết bị trước.")
        {
            IdThietBi = idThietBi;
        }
    }
}
