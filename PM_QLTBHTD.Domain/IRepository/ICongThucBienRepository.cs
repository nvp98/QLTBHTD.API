using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface ICongThucBienRepository : IGenericRepository<CBM_CongThuc_Bien>
    {
        Task<IEnumerable<CBM_CongThuc_Bien>> GetByCongThucAsync(int idCongThuc);
    }
}
