using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IChiTieuInputService
    {
        Task<IEnumerable<ChiTieuInputDto>> GetByChiTieuAsync(int idChiTieu);
        Task<ChiTieuInputDto?>             GetByIdAsync(int id);
        Task<ChiTieuInputDto>              CreateAsync(CreateChiTieuInputDto dto);
        Task<ChiTieuInputDto?>             UpdateAsync(int id, UpdateChiTieuInputDto dto);
        Task<bool>                         DeleteAsync(int id);
    }
}
