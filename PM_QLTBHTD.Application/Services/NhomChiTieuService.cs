using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class NhomChiTieuService : INhomChiTieuService
    {
        private readonly INhomChiTieuRepository _repository;
        private readonly IAppDbContext _db;

        public NhomChiTieuService(INhomChiTieuRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        private IQueryable<NhomChiTieuDto> JoinQuery()
        {
            return from n in _db.NhomChiTieus
                   join l in _db.LoaiThietBis on n.ID_LoaiThietBi equals l.ID_LoaiThietBi
                   select new NhomChiTieuDto
                   {
                       ID_NhomChiTieu = n.ID_NhomChiTieu,
                       TenNhom = n.TenNhom,
                       ID_LoaiThietBi = n.ID_LoaiThietBi,
                       TenLoaiThietBi = l.TenLoaiTB,
                       PhienBan = n.PhienBan,
                       TrangThai = n.TrangThai
                   };
        }

        public async Task<PagedResult<NhomChiTieuDto>> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = JoinQuery().Where(x =>
                string.IsNullOrEmpty(search)
                || x.TenNhom.Contains(search)
                || x.TenLoaiThietBi.Contains(search));

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<NhomChiTieuDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
        }

        public async Task<IEnumerable<NhomChiTieuDto>> GetAllActiveAsync()
            => await JoinQuery().Where(x => x.TrangThai == 1).ToListAsync();

        public async Task<IEnumerable<NhomChiTieuDto>> GetByLoaiThietBiAsync(int idLoaiThietBi)
            => await JoinQuery().Where(x => x.ID_LoaiThietBi == idLoaiThietBi).ToListAsync();

        public async Task<NhomChiTieuDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_NhomChiTieu == id);

        public async Task<NhomChiTieuDto> CreateAsync(CreateNhomChiTieuDto dto)
        {
            var entity = new CBM_NhomChiTieu
            {
                TenNhom = dto.TenNhom,
                ID_LoaiThietBi = dto.ID_LoaiThietBi,
                PhienBan = dto.PhienBan,
                TrangThai = dto.TrangThai
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (await GetByIdAsync(entity.ID_NhomChiTieu))!;
        }

        public async Task<NhomChiTieuDto?> UpdateAsync(int id, UpdateNhomChiTieuDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.TenNhom = dto.TenNhom;
            entity.ID_LoaiThietBi = dto.ID_LoaiThietBi;
            entity.PhienBan = dto.PhienBan;
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
