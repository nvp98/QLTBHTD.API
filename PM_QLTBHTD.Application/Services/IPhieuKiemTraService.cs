using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IPhieuKiemTraService
    {
        Task<PagedResult<PhieuKiemTraDto>> GetPagedAsync(string? search, int page, int? pageSize);
        Task<IEnumerable<PhieuKiemTraDto>> GetByThietBiAsync(int idThietBi);
        Task<IEnumerable<PhieuKiemTraDto>> GetByNgayAsync(DateTime tuNgay, DateTime denNgay);
        Task<PhieuKiemTraDetailDto?> GetDetailAsync(int idPhieu);
        /// <summary>Toàn bộ lịch sử đo (mọi phiếu) của 1 chỉ tiêu trên 1 thiết bị, mới nhất trước —
        /// dùng để xem xu hướng, KHÔNG liên quan tới fallback "lấy Si gần nhất" dùng khi tính điểm nhóm.</summary>
        Task<List<LichSuChiTieuDto>> GetLichSuChiTieuAsync(int idThietBi, int idChiTieu);
        Task<PhieuKiemTraDto?> GetByIdAsync(int id);
        Task<PhieuKiemTraDto> CreateAsync(CreatePhieuKiemTraDto dto);
        Task<PhieuKiemTraDto?> UpdateAsync(int id, UpdatePhieuKiemTraDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
