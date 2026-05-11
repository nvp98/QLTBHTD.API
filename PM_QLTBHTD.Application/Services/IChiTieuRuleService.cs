using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IChiTieuRuleService
    {
        Task<IEnumerable<ChiTieuRuleDto>> GetByChiTieuAsync(int idChiTieu);
        Task<ChiTieuRuleDto?>             GetByIdAsync(int id);
        Task<ChiTieuRuleDto>              CreateAsync(CreateChiTieuRuleDto dto);
        Task<ChiTieuRuleDto?>             UpdateAsync(int id, UpdateChiTieuRuleDto dto);
        Task<bool>                        DeleteAsync(int id);
    }
}
