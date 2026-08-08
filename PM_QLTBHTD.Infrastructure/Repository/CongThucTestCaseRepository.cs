using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class CongThucTestCaseRepository : GenericRepository<CBM_CongThuc_TestCase>, ICongThucTestCaseRepository
    {
        public CongThucTestCaseRepository(AppDbContext context) : base(context) { }

        public async Task<List<CBM_CongThuc_TestCase>> GetByCongThucAsync(int idCongThuc)
            => await _dbSet.Where(x => x.ID_CongThuc == idCongThuc).ToListAsync();
    }
}
