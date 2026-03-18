using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class KhuVucRepository : GenericRepository<CBM_KhuVuc>, IKhuVucRepository
    {
        public KhuVucRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_KhuVuc>> GetAllActiveAsync()
            => await _dbSet.Where(x => x.TrangThai == 1).ToListAsync();
    }
}
