using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface ILoaiThietBiService
    {
        Task<PagedResult<LoaiThietBiDto>> GetPagedAsync(string? search, int page, int pageSize);
        Task<IEnumerable<LoaiThietBiDto>> GetAllActiveAsync();
        Task<LoaiThietBiDto?> GetByIdAsync(int id);
        Task<LoaiThietBiDto> CreateAsync(CreateLoaiThietBiDto dto);
        Task<LoaiThietBiDto?> UpdateAsync(int id, UpdateLoaiThietBiDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
