using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface ITramDienService
    {
        Task<IEnumerable<TramDienDto>> GetAllAsync();
        Task<IEnumerable<TramDienDto>> GetAllActiveAsync();
        Task<IEnumerable<TramDienDto>> GetByKhuVucAsync(int idKhuVuc);
        Task<TramDienDto?> GetByIdAsync(int id);
        Task<TramDienDto> CreateAsync(CreateTramDienDto dto);
        Task<TramDienDto?> UpdateAsync(int id, UpdateTramDienDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
