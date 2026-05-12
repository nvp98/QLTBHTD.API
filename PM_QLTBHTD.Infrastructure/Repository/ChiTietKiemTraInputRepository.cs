using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class ChiTietKiemTraInputRepository : GenericRepository<ChiTietKiemTra_Input>, IChiTietKiemTraInputRepository
    {
        public ChiTietKiemTraInputRepository(AppDbContext context) : base(context) { }

        public async Task DeleteByPhieuAsync(int idPhieu)
        {
            var items = await _dbSet.Where(x => x.IDPhieu == idPhieu).ToListAsync();
            _dbSet.RemoveRange(items);
        }
    }
}
