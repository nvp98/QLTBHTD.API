using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class ThietBiRepository : GenericRepository<CBM_ThietBi>, IThietBiRepository
    {
        public ThietBiRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_ThietBi>> GetByTramAsync(int idTram)
            => await _dbSet.Where(x => x.ID_Tram == idTram).ToListAsync();

        public async Task<IEnumerable<CBM_ThietBi>> GetByLoaiThietBiAsync(int idLoaiTB)
            => await _dbSet.Where(x => x.ID_LoaiTB == idLoaiTB).ToListAsync();

        public async Task<IEnumerable<CBM_ThietBi>> GetAllActiveAsync()
            => await _dbSet.Where(x => x.TrangThai == 1).ToListAsync();
    }
}
