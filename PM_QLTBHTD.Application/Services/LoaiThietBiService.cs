using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class LoaiThietBiService : ILoaiThietBiService
    {
        private readonly ILoaiThietBiRepository _repository;

        public LoaiThietBiService(ILoaiThietBiRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<LoaiThietBiDto>> GetPagedAsync(string? search, int page, int pageSize)
        {
            var (items, total) = await _repository.GetPagedAsync(
                x => string.IsNullOrEmpty(search) || x.TenLoaiTB.Contains(search) || x.KyHieu.Contains(search),
                page, pageSize);

            return new PagedResult<LoaiThietBiDto>
            {
                Items = items.Select(MapToDto),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<LoaiThietBiDto>> GetAllActiveAsync()
        {
            var items = await _repository.GetAllActiveAsync();
            return items.Select(MapToDto);
        }

        public async Task<LoaiThietBiDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<LoaiThietBiDto> CreateAsync(CreateLoaiThietBiDto dto)
        {
            var entity = new CBM_LoaiThietBi
            {
                TenLoaiTB = dto.TenLoaiTB,
                KyHieu = dto.KyHieu,
                TrangThai = dto.TrangThai
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<LoaiThietBiDto?> UpdateAsync(int id, UpdateLoaiThietBiDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.TenLoaiTB = dto.TenLoaiTB;
            entity.KyHieu = dto.KyHieu;
            entity.TrangThai = dto.TrangThai;
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return true;
        }

        private static LoaiThietBiDto MapToDto(CBM_LoaiThietBi x) => new()
        {
            ID_LoaiThietBi = x.ID_LoaiThietBi,
            TenLoaiTB = x.TenLoaiTB,
            KyHieu = x.KyHieu,
            TrangThai = x.TrangThai
        };
    }
}
