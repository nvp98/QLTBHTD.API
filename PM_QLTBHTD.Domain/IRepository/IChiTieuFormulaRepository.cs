using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IChiTieuFormulaRepository : IGenericRepository<CBM_ChiTieu_Formula>
    {
        Task<IEnumerable<CBM_ChiTieu_Formula>> GetByChiTieuAsync(int idChiTieu);
    }
}
