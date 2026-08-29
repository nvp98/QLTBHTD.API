namespace PM_QLTBHTD.Application.Exceptions
{
    public class NhieuNhomGocException : Exception
    {
        public int IdLoaiThietBi { get; }
        public int SoLuongGoc { get; }

        public NhieuNhomGocException(int idLoaiThietBi, int soLuongGoc)
            : base($"Loại thiết bị ID={idLoaiThietBi} có {soLuongGoc} nhóm chỉ tiêu gốc (ID_NhomCha=null) — " +
                   "không thể tự động xác định nhóm nào là CSSK chính thức. Vào Cây chỉ tiêu sửa lại " +
                   "quan hệ cha-con để chỉ còn đúng 1 nhóm gốc.")
        {
            IdLoaiThietBi = idLoaiThietBi;
            SoLuongGoc = soLuongGoc;
        }
    }
}
