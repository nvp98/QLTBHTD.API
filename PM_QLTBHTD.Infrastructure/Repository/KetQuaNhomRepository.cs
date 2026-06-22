using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;

namespace PM_QLTBHTD.Infrastructure.Repository
{
    public class KetQuaNhomRepository : GenericRepository<CBM_KetQuaNhom>, IKetQuaNhomRepository
    {
        public KetQuaNhomRepository(AppDbContext context) : base(context) { }

        public async Task<CBM_KetQuaNhom?> GetByPhieuNhomAsync(int idPhieu, int idNhomChiTieu)
            => await _dbSet.FirstOrDefaultAsync(x => x.IDPhieu == idPhieu && x.ID_NhomChiTieu == idNhomChiTieu);

        public async Task<IEnumerable<CBM_KetQuaNhom>> GetByPhieuAsync(int idPhieu)
            => await _dbSet.Where(x => x.IDPhieu == idPhieu).ToListAsync();

        public async Task UpsertAsync(CBM_KetQuaNhom ketQua)
        {
            var existing = await GetByPhieuNhomAsync(ketQua.IDPhieu, ketQua.ID_NhomChiTieu);
            if (existing != null)
            {
                existing.Diem = ketQua.Diem;
                existing.BienDaBind = ketQua.BienDaBind;
                existing.ThoiGianTinh = ketQua.ThoiGianTinh;
                _dbSet.Update(existing);
            }
            else
            {
                await _dbSet.AddAsync(ketQua);
            }
        }
    }
}
