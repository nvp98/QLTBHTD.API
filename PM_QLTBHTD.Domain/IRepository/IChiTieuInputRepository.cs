using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface IChiTieuInputRepository : IGenericRepository<CBM_ChiTieu_Input>
    {
        Task<IEnumerable<CBM_ChiTieu_Input>> GetByChiTieuAsync(int idChiTieu);
    }
}
