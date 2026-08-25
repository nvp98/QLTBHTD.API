using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface INganLoService
    {
        Task<PagedResult<NganLoDto>> GetPagedAsync(string? search, int page, int? pageSize);
        Task<IEnumerable<NganLoDto>> GetAllActiveAsync();
        Task<IEnumerable<NganLoDto>> GetByTramAsync(int idTram);
        Task<NganLoDto?> GetByIdAsync(int id);
        Task<NganLoDto> CreateAsync(CreateNganLoDto dto);
        Task<NganLoDto?> UpdateAsync(int id, UpdateNganLoDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
