using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Helpers;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface INguongService
    {
        Task<PagedResult<NguongDto>> GetPagedAsync(string? search, int page, int? pageSize);
        Task<IEnumerable<NguongDto>> GetByChiTieuAsync(int idChiTieu);
        Task<NguongDto?> GetByIdAsync(int id);
        Task<NguongDto> CreateAsync(CreateNguongDto dto);
        Task<NguongDto?> UpdateAsync(int id, UpdateNguongDto dto);
        Task<bool> DeleteAsync(int id);
        /// <summary>Config Validator — quét gap/overlap trong bảng ngưỡng của 1 chỉ tiêu.</summary>
        Task<List<NguongValidationIssue>> ValidateGapOverlapAsync(int idChiTieu);
    }
}
