using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class KetQuaPhanLoaiThangRepository : GenericRepository<CBM_KetQuaPhanLoaiThang>, IKetQuaPhanLoaiThangRepository
    {
        public KetQuaPhanLoaiThangRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_KetQuaPhanLoaiThang>> GetByPhieuChiTieuAsync(int idPhieu, int idChiTieu)
            => await _dbSet
                .Where(x => x.IDPhieu == idPhieu && x.ID_ChiTieu == idChiTieu)
                .OrderBy(x => x.Nam).ThenBy(x => x.Thang)
                .ToListAsync();
    }
}
