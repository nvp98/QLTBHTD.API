using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class NguongService : INguongService
    {
        private readonly INguongRepository _repository;
        private readonly IAppDbContext _db;

        public NguongService(INguongRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        private IQueryable<NguongDto> JoinQuery()
        {
            return from ng in _db.Nguongs
                   join c in _db.ChiTieus on ng.ID_ChiTieu equals c.ID_ChiTieu
                   select new NguongDto
                   {
                       ID_Nguong = ng.ID_Nguong,
                       ID_ChiTieu = ng.ID_ChiTieu,
                       TenChiTieu = c.TenChiTieu,
                       CanTren = ng.CanTren,
                       CanDuoi = ng.CanDuoi,
                       Diem_Si = ng.Diem_Si
                   };
        }

        public async Task<PagedResult<NguongDto>> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = JoinQuery().Where(x =>
                string.IsNullOrEmpty(search)
                || x.TenChiTieu.Contains(search));

            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<NguongDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
        }

        public async Task<IEnumerable<NguongDto>> GetByChiTieuAsync(int idChiTieu)
            => await JoinQuery().Where(x => x.ID_ChiTieu == idChiTieu).ToListAsync();

        public async Task<NguongDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_Nguong == id);

        public async Task<NguongDto> CreateAsync(CreateNguongDto dto)
        {
            var entity = new CBM_Nguong
            {
                ID_ChiTieu = dto.ID_ChiTieu,
                CanTren = dto.CanTren,
                CanDuoi = dto.CanDuoi,
                Diem_Si = dto.Diem_Si
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return (await GetByIdAsync(entity.ID_Nguong))!;
        }

        public async Task<NguongDto?> UpdateAsync(int id, UpdateNguongDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_ChiTieu = dto.ID_ChiTieu;
            entity.CanTren = dto.CanTren;
            entity.CanDuoi = dto.CanDuoi;
            entity.Diem_Si = dto.Diem_Si;
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
