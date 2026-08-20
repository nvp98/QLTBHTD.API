using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface ICongThucBienService
    {
        Task<IEnumerable<CongThucBienDto>> GetByCongThucAsync(int idCongThuc);
        Task<CongThucBienDto?> GetByIdAsync(int id);
        Task<CongThucBienDto> CreateAsync(CreateCongThucBienDto dto);
        Task<CongThucBienDto?> UpdateAsync(int id, UpdateCongThucBienDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
