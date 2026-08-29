using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IChiTieuFormulaThamSoRepository : IGenericRepository<CBM_ChiTieu_Formula_ThamSo>
    {
        Task<IEnumerable<CBM_ChiTieu_Formula_ThamSo>> GetByFormulaAsync(int idFormula);
    }
}
