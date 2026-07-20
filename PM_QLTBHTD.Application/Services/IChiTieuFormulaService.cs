using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services
{
    public interface IChiTieuFormulaService
    {
        Task<IEnumerable<ChiTieuFormulaDto>> GetByChiTieuAsync(int idChiTieu);
        Task<ChiTieuFormulaDto?>             GetByIdAsync(int id);
        Task<ChiTieuFormulaDto>              CreateAsync(CreateChiTieuFormulaDto dto);
        Task<ChiTieuFormulaDto?>             UpdateAsync(int id, UpdateChiTieuFormulaDto dto);
        Task<bool>                           DeleteAsync(int id);
    }
}
