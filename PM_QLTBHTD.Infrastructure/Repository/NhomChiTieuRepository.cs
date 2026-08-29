using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class NhomChiTieuRepository : GenericRepository<CBM_NhomChiTieu>, INhomChiTieuRepository
    {
        public NhomChiTieuRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<CBM_NhomChiTieu>> GetByLoaiThietBiAsync(int idLoaiThietBi)
            => await _dbSet.Where(x => x.ID_LoaiThietBi == idLoaiThietBi).ToListAsync();

        public async Task<IEnumerable<CBM_NhomChiTieu>> GetAllActiveAsync()
            => await _dbSet.Where(x => x.TrangThai == 1).ToListAsync();
    }
}
