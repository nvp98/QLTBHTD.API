using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IChiTieuRuleRepository : IGenericRepository<CBM_ChiTieu_Rule>
    {
        Task<IEnumerable<CBM_ChiTieu_Rule>> GetByChiTieuAsync(int idChiTieu);
    }
}
