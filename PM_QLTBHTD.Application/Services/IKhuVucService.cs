using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IKhuVucService
    {
        Task<IEnumerable<KhuVucDto>> GetAllAsync();
        Task<IEnumerable<KhuVucDto>> GetAllActiveAsync();
        Task<KhuVucDto?> GetByIdAsync(int id);
        Task<KhuVucDto> CreateAsync(CreateKhuVucDto dto);
        Task<KhuVucDto?> UpdateAsync(int id, UpdateKhuVucDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
