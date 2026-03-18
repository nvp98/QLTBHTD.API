using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IPhieuKiemTraService
    {
        Task<IEnumerable<PhieuKiemTraDto>> GetAllAsync();
        Task<IEnumerable<PhieuKiemTraDto>> GetByThietBiAsync(int idThietBi);
        Task<IEnumerable<PhieuKiemTraDto>> GetByNgayAsync(DateTime tuNgay, DateTime denNgay);
        Task<PhieuKiemTraDetailDto?> GetDetailAsync(int idPhieu);
        Task<PhieuKiemTraDto?> GetByIdAsync(int id);
        Task<PhieuKiemTraDto> CreateAsync(CreatePhieuKiemTraDto dto);
        Task<PhieuKiemTraDto?> UpdateAsync(int id, UpdatePhieuKiemTraDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
