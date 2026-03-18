using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class PhieuKiemTraRepository : GenericRepository<PhieuKiemTra>, IPhieuKiemTraRepository
    {
        public PhieuKiemTraRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<PhieuKiemTra>> GetByThietBiAsync(int idThietBi)
            => await _dbSet.Where(x => x.ID_ThietBi == idThietBi)
                           .OrderByDescending(x => x.NgayKiemTra)
                           .ToListAsync();

        public async Task<IEnumerable<PhieuKiemTra>> GetByNgayKiemTraAsync(DateTime tuNgay, DateTime denNgay)
            => await _dbSet.Where(x => x.NgayKiemTra >= tuNgay && x.NgayKiemTra <= denNgay)
                           .OrderByDescending(x => x.NgayKiemTra)
                           .ToListAsync();

        public async Task<PhieuKiemTra?> GetWithChiTietAsync(int idPhieu)
            => await _dbSet.FirstOrDefaultAsync(x => x.ID_Phieu == idPhieu);
    }
}
