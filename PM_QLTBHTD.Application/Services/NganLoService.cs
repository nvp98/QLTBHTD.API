using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class NganLoService : INganLoService
    {
        private readonly INganLoRepository _repository;
        private readonly IAppDbContext _db;

        public NganLoService(INganLoRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        private IQueryable<NganLoDto> JoinQuery()
        {
            return from n in _db.NganLos
                   join t in _db.TramDiens on n.ID_Tram equals t.IDTram
                   select new NganLoDto
                   {
                       ID_NganLo = n.ID_NganLo,
                       ID_Tram = n.ID_Tram,
                       TenTram = t.TenTram,
                       TenNganLo = n.TenNganLo,
                       MaNganLo = n.MaNganLo,
                       TrangThai = n.TrangThai,
                       SoThietBi = _db.ThietBis.Count(x => x.ID_NganLo == n.ID_NganLo)
                   };
        }

        public async Task<PagedResult<NganLoDto>> GetPagedAsync(string? search, int page, int? pageSize)
        {
            var query = JoinQuery().OrderBy(x => x.TenTram).ThenBy(x => x.TenNganLo).AsQueryable();
            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.TenNganLo.Contains(search) || x.TenTram.Contains(search));

            var total = await query.CountAsync();
            if (pageSize.HasValue)
                query = query.Skip((page - 1) * pageSize.Value).Take(pageSize.Value);
            var items = await query.ToListAsync();
            return new PagedResult<NganLoDto> { Items = items, Total = total, Page = page, PageSize = pageSize ?? total };
        }

        public async Task<IEnumerable<NganLoDto>> GetAllActiveAsync()
            => await JoinQuery().Where(x => x.TrangThai == 1).OrderBy(x => x.TenTram).ThenBy(x => x.TenNganLo).ToListAsync();

        public async Task<IEnumerable<NganLoDto>> GetByTramAsync(int idTram)
            => await JoinQuery().Where(x => x.ID_Tram == idTram).OrderBy(x => x.TenNganLo).ToListAsync();

        public async Task<NganLoDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_NganLo == id);

        public async Task<NganLoDto> CreateAsync(CreateNganLoDto dto)
        {
            var entity = new CBM_NganLo
            {
                ID_Tram = dto.ID_Tram,
                TenNganLo = dto.TenNganLo,
                MaNganLo = dto.MaNganLo,
                TrangThai = dto.TrangThai
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (await GetByIdAsync(entity.ID_NganLo))!;
        }

        public async Task<NganLoDto?> UpdateAsync(int id, UpdateNganLoDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_Tram = dto.ID_Tram;
            entity.TenNganLo = dto.TenNganLo;
            entity.MaNganLo = dto.MaNganLo;
            entity.TrangThai = dto.TrangThai;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            var soThietBiDangDung = await _db.ThietBis.CountAsync(x => x.ID_NganLo == id);
            if (soThietBiDangDung > 0)
                throw new NganLoDangSuDungException(id, soThietBiDangDung);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
