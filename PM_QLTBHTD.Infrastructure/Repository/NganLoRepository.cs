using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class NganLoRepository : GenericRepository<CBM_NganLo>, INganLoRepository
    {
        public NganLoRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_NganLo>> GetByTramAsync(int idTram)
            => await _dbSet.Where(x => x.ID_Tram == idTram).ToListAsync();

        public async Task<IEnumerable<CBM_NganLo>> GetAllActiveAsync()
            => await _dbSet.Where(x => x.TrangThai == 1).ToListAsync();
    }
}
