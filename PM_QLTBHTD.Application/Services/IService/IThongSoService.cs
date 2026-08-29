using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface IThongSoService
    {
        Task<List<ThongSoDto>> GetAllAsync();
        Task<ThongSoDto?> GetByIdAsync(int id);
        Task<ThongSoDto> CreateAsync(CreateThongSoDto dto);
        Task<ThongSoDto?> UpdateAsync(int id, UpdateThongSoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
