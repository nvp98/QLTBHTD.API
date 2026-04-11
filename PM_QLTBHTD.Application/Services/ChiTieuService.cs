using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuService : IChiTieuService
    {
        private readonly IChiTieuRepository _repository;
        private readonly IAppDbContext _db;

        public ChiTieuService(IChiTieuRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }
        private IQueryable<ChiTieuDto> JoinQuery()
        {
            return from c in _db.ChiTieus.AsNoTracking()
                   join n in _db.NhomChiTieus.AsNoTracking()
                       on c.ID_NhomChiTieu equals n.ID_NhomChiTieu into gj
                   from n in gj.DefaultIfEmpty()
                   join l in _db.LoaiThietBis.AsNoTracking()
                       on (n != null ? n.ID_LoaiThietBi : 0) equals l.ID_LoaiThietBi into lj
                   from l in lj.DefaultIfEmpty()
                   select new ChiTieuDto
                   {
                       ID_ChiTieu     = c.ID_ChiTieu,
                       ID_NhomChiTieu = c.ID_NhomChiTieu,
                       TenNhom        = n != null ? n.TenNhom : "",
                       ID_LoaiThietBi = n != null ? n.ID_LoaiThietBi : 0,
                       TenChiTieu     = c.TenChiTieu,
                       TrongSo_Wi     = c.TrongSo_Wi,
                       TrangThai      = c.TrangThai
                   };
        }

        public async Task<PagedResult<ChiTieuDto>> GetPagedAsync(string? search, int? idNhom, int? idLoai, int page, int pageSize)
        {
            var query = JoinQuery();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(x =>
                    x.TenChiTieu.Contains(search) || x.TenNhom.Contains(search));

            if (idNhom.HasValue)
                query = query.Where(x => x.ID_NhomChiTieu == idNhom.Value);

            if (idLoai.HasValue)
                query = query.Where(x => x.ID_LoaiThietBi == idLoai.Value);

            var total = await query.CountAsync();
            var items = await query.OrderBy(x => x.ID_NhomChiTieu).ThenBy(x => x.ID_ChiTieu)
                                   .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<ChiTieuDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
        }

        public async Task<IEnumerable<ChiTieuDto>> GetAllActiveAsync()
            => await JoinQuery().Where(x => x.TrangThai == 1).ToListAsync();

        public async Task<IEnumerable<ChiTieuDto>> GetByNhomChiTieuAsync(int idNhomChiTieu)
            => await JoinQuery().Where(x => x.ID_NhomChiTieu == idNhomChiTieu).ToListAsync();

        public async Task<ChiTieuDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_ChiTieu == id);

        public async Task<ChiTieuDto> CreateAsync(CreateChiTieuDto dto)
        {
            var entity = new CBM_ChiTieu
            {
                ID_NhomChiTieu = dto.ID_NhomChiTieu,
                TenChiTieu = dto.TenChiTieu,
                TrongSo_Wi = dto.TrongSo_Wi,
                TrangThai = dto.TrangThai
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (await GetByIdAsync(entity.ID_ChiTieu))!;
        }

        public async Task<ChiTieuDto?> UpdateAsync(int id, UpdateChiTieuDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_NhomChiTieu = dto.ID_NhomChiTieu;
            entity.TenChiTieu = dto.TenChiTieu;
            entity.TrongSo_Wi = dto.TrongSo_Wi;
            entity.TrangThai = dto.TrangThai;
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
