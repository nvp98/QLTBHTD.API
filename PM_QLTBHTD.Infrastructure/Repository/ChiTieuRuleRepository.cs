using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class ChiTieuRuleRepository : GenericRepository<CBM_ChiTieu_Rule>, IChiTieuRuleRepository
    {
        public ChiTieuRuleRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_ChiTieu_Rule>> GetByChiTieuAsync(int idChiTieu)
            => await _dbSet.Where(x => x.ID_ChiTieu == idChiTieu).ToListAsync();
    }
}
