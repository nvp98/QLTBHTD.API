using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class ThietBiThongSoRepository : GenericRepository<CBM_ThietBi_ThongSo>, IThietBiThongSoRepository
    {
        public ThietBiThongSoRepository(AppDbContext context) : base(context) { }

        public async Task<List<CBM_ThietBi_ThongSo>> GetByThietBiAsync(int idThietBi)
            => await _dbSet.Where(x => x.ID_ThietBi == idThietBi).ToListAsync();

        public async Task<List<CBM_ThietBi_ThongSo>> GetByThongSoAsync(int idThongSo)
            => await _dbSet.Where(x => x.ID_ThongSo == idThongSo).ToListAsync();
    }
}
