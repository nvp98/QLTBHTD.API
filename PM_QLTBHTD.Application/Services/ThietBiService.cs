using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ThietBiService : IThietBiService
    {
        private readonly IThietBiRepository _repository;
        private readonly IAppDbContext _db;

        public ThietBiService(IThietBiRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        private IQueryable<ThietBiDto> JoinQuery()
        {
            return from tb in _db.ThietBis
                   join t in _db.TramDiens on tb.ID_Tram equals t.IDTram into tramGroup
                   from t in tramGroup.DefaultIfEmpty()
                   join l in _db.LoaiThietBis on tb.ID_LoaiTB equals l.ID_LoaiThietBi into loaiGroup
                   from l in loaiGroup.DefaultIfEmpty()
                   select new ThietBiDto
                   {
                       ID_ThietBi = tb.ID_ThietBi,
                       ID_Tram = tb.ID_Tram,
                       TenTram = t != null ? t.TenTram : string.Empty,
                       ID_LoaiTB = tb.ID_LoaiTB,
                       TenLoaiTB = l != null ? l.TenLoaiTB : string.Empty,
                       TenThietBi = tb.TenThietBi,
                       SoHieu = tb.SoHieu,
                       NhanHieu = tb.NhanHieu,
                       NamSanXuat = tb.NamSanXuat,
                       TrangThai = tb.TrangThai,
                       GhiChu = tb.GhiChu
                   };
        }

        public async Task<PagedResult<ThietBiDto>> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = JoinQuery().Where(x =>
                string.IsNullOrEmpty(search)
                || x.TenThietBi.Contains(search)
                || (x.SoHieu != null && x.SoHieu.Contains(search))
                || (!string.IsNullOrEmpty(x.TenTram) && x.TenTram.Contains(search)));

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<ThietBiDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
        }

        public async Task<IEnumerable<ThietBiDto>> GetAllActiveAsync()
            => await JoinQuery().Where(x => x.TrangThai == 1).ToListAsync();

        public async Task<IEnumerable<ThietBiDto>> GetByTramAsync(int idTram)
            => await JoinQuery().Where(x => x.ID_Tram == idTram).ToListAsync();

        public async Task<IEnumerable<ThietBiDto>> GetByLoaiThietBiAsync(int idLoaiTB)
            => await JoinQuery().Where(x => x.ID_LoaiTB == idLoaiTB).ToListAsync();

        public async Task<ThietBiDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_ThietBi == id);

        public async Task<ThietBiDto> CreateAsync(CreateThietBiDto dto)
        {
            var entity = new CBM_ThietBi
            {
                ID_Tram = dto.ID_Tram,
                ID_LoaiTB = dto.ID_LoaiTB,
                TenThietBi = dto.TenThietBi,
                SoHieu = dto.SoHieu,
                NhanHieu = dto.NhanHieu,
                NamSanXuat = dto.NamSanXuat,
                TrangThai = dto.TrangThai,
                GhiChu = dto.GhiChu
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (await GetByIdAsync(entity.ID_ThietBi))!;
        }

        public async Task<ThietBiDto?> UpdateAsync(int id, UpdateThietBiDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_Tram = dto.ID_Tram;
            entity.ID_LoaiTB = dto.ID_LoaiTB;
            entity.TenThietBi = dto.TenThietBi;
            entity.SoHieu = dto.SoHieu;
            entity.NhanHieu = dto.NhanHieu;
            entity.NamSanXuat = dto.NamSanXuat;
            entity.TrangThai = dto.TrangThai;
            entity.GhiChu = dto.GhiChu;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
