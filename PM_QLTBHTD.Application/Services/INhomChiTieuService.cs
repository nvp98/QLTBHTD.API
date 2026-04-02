using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface INhomChiTieuService
    {
        Task<PagedResult<NhomChiTieuDto>> GetPagedAsync(string? search, int page, int pageSize);
        Task<IEnumerable<NhomChiTieuDto>> GetAllActiveAsync();
        Task<IEnumerable<NhomChiTieuDto>> GetByLoaiThietBiAsync(int idLoaiThietBi);
        Task<NhomChiTieuDto?> GetByIdAsync(int id);
        Task<NhomChiTieuDto> CreateAsync(CreateNhomChiTieuDto dto);
        Task<NhomChiTieuDto?> UpdateAsync(int id, UpdateNhomChiTieuDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
