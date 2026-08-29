using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class CongThucBienRepository : GenericRepository<CBM_CongThuc_Bien>, ICongThucBienRepository
    {
        public CongThucBienRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_CongThuc_Bien>> GetByCongThucAsync(int idCongThuc)
            => await _dbSet.Where(x => x.ID_CongThuc == idCongThuc).ToListAsync();
    }
}
