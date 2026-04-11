using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IChiTieuService
    {
        Task<PagedResult<ChiTieuDto>> GetPagedAsync(string? search, int? idNhom, int? idLoai, int page, int pageSize);
        Task<IEnumerable<ChiTieuDto>> GetAllActiveAsync();
        Task<IEnumerable<ChiTieuDto>> GetByNhomChiTieuAsync(int idNhomChiTieu);
        Task<ChiTieuDto?> GetByIdAsync(int id);
        Task<ChiTieuDto> CreateAsync(CreateChiTieuDto dto);
        Task<ChiTieuDto?> UpdateAsync(int id, UpdateChiTieuDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
