using PM_QLTBHTD.Application.DTOs;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface IChiTieuFormulaThamSoService
    {
        Task<IEnumerable<ChiTieuFormulaThamSoDto>> GetByFormulaAsync(int idFormula);
        Task<ChiTieuFormulaThamSoDto?>             GetByIdAsync(int id);
        Task<ChiTieuFormulaThamSoDto>              CreateAsync(CreateChiTieuFormulaThamSoDto dto);
        Task<ChiTieuFormulaThamSoDto?>             UpdateAsync(int id, UpdateChiTieuFormulaThamSoDto dto);
        Task<bool>                                 DeleteAsync(int id);
    }
}
