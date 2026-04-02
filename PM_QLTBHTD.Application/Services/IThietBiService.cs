using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IThietBiService
    {
        Task<PagedResult<ThietBiDto>> GetPagedAsync(string? search, int page, int pageSize);
        Task<IEnumerable<ThietBiDto>> GetAllActiveAsync();
        Task<IEnumerable<ThietBiDto>> GetByTramAsync(int idTram);
        Task<IEnumerable<ThietBiDto>> GetByLoaiThietBiAsync(int idLoaiTB);
        Task<ThietBiDto?> GetByIdAsync(int id);
        Task<ThietBiDto> CreateAsync(CreateThietBiDto dto);
        Task<ThietBiDto?> UpdateAsync(int id, UpdateThietBiDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
