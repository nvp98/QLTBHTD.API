using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    /// <summary>
    /// Tính Diem_Si_DatDuoc cho tất cả chỉ tiêu lá của một phiếu,
    /// ghi kết quả vào ChiTietKiemTra trước khi ScoringEngine được gọi.
    /// </summary>
    public interface IChiTieuScoringService
    {
        /// <summary>
        /// Nhận batch dữ liệu nhập từ form, tính Si cho từng chỉ tiêu và lưu vào DB.
        /// Gọi trước TinhDiemNhomAsync.
        /// </summary>
        Task TinhVaLuuDiemSiAsync(int idPhieu, IEnumerable<NhapChiTietKiemTraDto> danhSachNhap, CancellationToken ct = default);
    }
}
