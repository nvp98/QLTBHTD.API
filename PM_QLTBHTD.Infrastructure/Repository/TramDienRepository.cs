using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class TramDienRepository : GenericRepository<CBM_TramDien>, ITramDienRepository
    {
        public TramDienRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_TramDien>> GetByKhuVucAsync(int idKhuVuc)
            => await _dbSet.Where(x => x.IDKhuVuc == idKhuVuc).ToListAsync();

        public async Task<IEnumerable<CBM_TramDien>> GetAllActiveAsync()
            => await _dbSet.Where(x => x.TrangThai == 1).ToListAsync();
    }
}
