using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class TramDienService : ITramDienService
    {
        private readonly ITramDienRepository _repository;
        private readonly IAppDbContext _db;

        public TramDienService(ITramDienRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<PagedResult<TramDienDto>> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = from t in _db.TramDiens
                        join k in _db.KhuVucs on t.IDKhuVuc equals k.ID_KhuVuc
                        where string.IsNullOrEmpty(search)
                              || t.TenTram.Contains(search)
                              || (t.DiaDiem != null && t.DiaDiem.Contains(search))
                        select new TramDienDto
                        {
                            IDTram = t.IDTram,
                            IDKhuVuc = t.IDKhuVuc,
                            TenKhuVuc = k.TenKhuVuc,
                            TenTram = t.TenTram,
                            DiaDiem = t.DiaDiem,
                            TrangThai = t.TrangThai
                        };

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<TramDienDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
        }

        public async Task<IEnumerable<TramDienDto>> GetAllActiveAsync()
        {
            return await (from t in _db.TramDiens
                          join k in _db.KhuVucs on t.IDKhuVuc equals k.ID_KhuVuc
                          where t.TrangThai == 1
                          select new TramDienDto
                          {
                              IDTram = t.IDTram,
                              IDKhuVuc = t.IDKhuVuc,
                              TenKhuVuc = k.TenKhuVuc,
                              TenTram = t.TenTram,
                              DiaDiem = t.DiaDiem,
                              TrangThai = t.TrangThai
                          }).ToListAsync();
        }

        public async Task<IEnumerable<TramDienDto>> GetByKhuVucAsync(int idKhuVuc)
        {
            return await (from t in _db.TramDiens
                          join k in _db.KhuVucs on t.IDKhuVuc equals k.ID_KhuVuc
                          where t.IDKhuVuc == idKhuVuc
                          select new TramDienDto
                          {
                              IDTram = t.IDTram,
                              IDKhuVuc = t.IDKhuVuc,
                              TenKhuVuc = k.TenKhuVuc,
                              TenTram = t.TenTram,
                              DiaDiem = t.DiaDiem,
                              TrangThai = t.TrangThai
                          }).ToListAsync();
        }

        public async Task<TramDienDto?> GetByIdAsync(int id)
        {
            return await (from t in _db.TramDiens
                          join k in _db.KhuVucs on t.IDKhuVuc equals k.ID_KhuVuc
                          where t.IDTram == id
                          select new TramDienDto
                          {
                              IDTram = t.IDTram,
                              IDKhuVuc = t.IDKhuVuc,
                              TenKhuVuc = k.TenKhuVuc,
                              TenTram = t.TenTram,
                              DiaDiem = t.DiaDiem,
                              TrangThai = t.TrangThai
                          }).FirstOrDefaultAsync();
        }

        public async Task<TramDienDto> CreateAsync(CreateTramDienDto dto)
        {
            var entity = new CBM_TramDien
            {
                IDKhuVuc = dto.IDKhuVuc,
                TenTram = dto.TenTram,
                DiaDiem = dto.DiaDiem,
                TrangThai = dto.TrangThai
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (await GetByIdAsync(entity.IDTram))!;
        }

        public async Task<TramDienDto?> UpdateAsync(int id, UpdateTramDienDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.IDKhuVuc = dto.IDKhuVuc;
            entity.TenTram = dto.TenTram;
            entity.DiaDiem = dto.DiaDiem;
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
