using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class KhuVucService : IKhuVucService
    {
        private readonly IKhuVucRepository _repository;
        private readonly IAppDbContext _db;

        public KhuVucService(IKhuVucRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        public async Task<PagedResult<KhuVucDto>> GetPagedAsync(string? search, int page, int? pageSize)
        {
            var (items, total) = await _repository.GetPagedAsync(
                x => string.IsNullOrEmpty(search) || x.TenKhuVuc.Contains(search),
                page, pageSize);

            return new PagedResult<KhuVucDto>
            {
                Items = items.Select(MapToDto),
                Total = total,
                Page = page,
                PageSize = pageSize ?? total
            };
        }

        public async Task<IEnumerable<KhuVucDto>> GetAllActiveAsync()
        {
            var items = await _repository.GetAllActiveAsync();
            return items.Select(MapToDto);
        }

        public async Task<KhuVucDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<KhuVucDto> CreateAsync(CreateKhuVucDto dto)
        {
            var entity = new CBM_KhuVuc
            {
                TenKhuVuc = dto.TenKhuVuc,
                TrangThai = dto.TrangThai
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<KhuVucDto?> UpdateAsync(int id, UpdateKhuVucDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.TenKhuVuc = dto.TenKhuVuc;
            entity.TrangThai = dto.TrangThai;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            var soTramDangDung = await _db.TramDiens.CountAsync(x => x.IDKhuVuc == id);
            if (soTramDangDung > 0)
                throw new KhuVucDangSuDungException(id, soTramDangDung);

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        private static KhuVucDto MapToDto(CBM_KhuVuc x) => new()
        {
            ID_KhuVuc = x.ID_KhuVuc,
            TenKhuVuc = x.TenKhuVuc,
            TrangThai = x.TrangThai
        };
    }
}
