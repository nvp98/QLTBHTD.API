using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface INguongService
    {
        Task<PagedResult<NguongDto>> GetPagedAsync(string? search, int page, int pageSize);
        Task<IEnumerable<NguongDto>> GetByChiTieuAsync(int idChiTieu);
        Task<NguongDto?> GetByIdAsync(int id);
        Task<NguongDto> CreateAsync(CreateNguongDto dto);
        Task<NguongDto?> UpdateAsync(int id, UpdateNguongDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
