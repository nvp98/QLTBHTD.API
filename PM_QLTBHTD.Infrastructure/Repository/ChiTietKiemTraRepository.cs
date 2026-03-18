using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class ChiTietKiemTraRepository : GenericRepository<ChiTietKiemTra>, IChiTietKiemTraRepository
    {
        public ChiTietKiemTraRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ChiTietKiemTra>> GetByPhieuAsync(int idPhieu)
            => await _dbSet.Where(x => x.IDPhieu == idPhieu).ToListAsync();

        public async Task DeleteByPhieuAsync(int idPhieu)
        {
            var items = await _dbSet.Where(x => x.IDPhieu == idPhieu).ToListAsync();
            _dbSet.RemoveRange(items);
        }
    }
}
