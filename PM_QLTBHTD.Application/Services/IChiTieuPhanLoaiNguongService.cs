using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    /// <summary>CRUD cho các mức phân loại (N0..N4) của chỉ tiêu kiểu LoaiTinhDiem='LF'.</summary>
    public interface IChiTieuPhanLoaiNguongService
    {
        Task<IEnumerable<ChiTieuPhanLoaiNguongDto>> GetByChiTieuAsync(int idChiTieu);
        Task<ChiTieuPhanLoaiNguongDto?> GetByIdAsync(int id);
        Task<ChiTieuPhanLoaiNguongDto> CreateAsync(CreateChiTieuPhanLoaiNguongDto dto);
        Task<ChiTieuPhanLoaiNguongDto?> UpdateAsync(int id, UpdateChiTieuPhanLoaiNguongDto dto);
        Task<bool> DeleteAsync(int id);

        /// <summary>Kết quả phân loại từng tháng đã tính cho 1 (phiếu, chỉ tiêu) — dùng để hiển thị lại sau khi nhập.</summary>
        Task<IEnumerable<KetQuaPhanLoaiThangDto>> GetKetQuaThangAsync(int idPhieu, int idChiTieu);
    }
}
