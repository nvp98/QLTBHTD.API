using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Domain.IRepository
{
    public interface ICongThucTestCaseRepository : IGenericRepository<CBM_CongThuc_TestCase>
    {
        Task<List<CBM_CongThuc_TestCase>> GetByCongThucAsync(int idCongThuc);
    }
}
