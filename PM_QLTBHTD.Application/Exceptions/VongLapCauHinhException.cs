namespace PM_QLTBHTD.Application.Exceptions
{
    public class VongLapCauHinhException : Exception
    {
        public int TuNhom { get; }
        public int DenNhom { get; }

        public VongLapCauHinhException(int tuNhom, int denNhom)
            : base($"Không thể lưu: Nhóm ID={tuNhom} tham chiếu tới Nhóm ID={denNhom} sẽ tạo VÒNG LẶP " +
                   $"trong cây công thức tổng hợp (Nhóm ID={denNhom} đã có đường tham chiếu ngược về Nhóm ID={tuNhom}).")
        {
            TuNhom = tuNhom;
            DenNhom = denNhom;
        }
    }
}
