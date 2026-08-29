using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class NguongRepository : GenericRepository<CBM_Nguong>, INguongRepository
    {
        public NguongRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_Nguong>> GetByChiTieuAsync(int idChiTieu)
            => await _dbSet.Where(x => x.ID_ChiTieu == idChiTieu).ToListAsync();
    }
}
