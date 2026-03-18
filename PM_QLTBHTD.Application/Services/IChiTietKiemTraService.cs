using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IChiTietKiemTraService
    {
        Task<IEnumerable<ChiTietKiemTraDto>> GetByPhieuAsync(int idPhieu);
        Task<ChiTietKiemTraDto?> GetByIdAsync(int id);
        Task<ChiTietKiemTraDto> CreateAsync(int idPhieu, CreateChiTietKiemTraDto dto);
        Task<ChiTietKiemTraDto?> UpdateAsync(int id, UpdateChiTietKiemTraDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
