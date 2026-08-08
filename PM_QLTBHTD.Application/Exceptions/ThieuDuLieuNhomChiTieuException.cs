namespace PM_QLTBHTD.Application.Exceptions
{
    public class ThieuDuLieuNhomChiTieuException : Exception
    {
        public int IdNhomChiTieu { get; }

        public ThieuDuLieuNhomChiTieuException(int idNhomChiTieu)
            : base($"Nhóm chỉ tiêu ID={idNhomChiTieu} chưa có chỉ tiêu nào có điểm Sᵢ " +
                   "(chưa đo hoặc nhóm chưa cấu hình chỉ tiêu). Không thể tính điểm nhóm.")
        {
            IdNhomChiTieu = idNhomChiTieu;
        }
    }
}
