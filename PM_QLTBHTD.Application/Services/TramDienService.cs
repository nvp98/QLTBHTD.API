using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class TramDienService : ITramDienService
    {
        private readonly ITramDienRepository _repository;

        public TramDienService(ITramDienRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TramDienDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<TramDienDto>> GetAllActiveAsync()
        {
            var items = await _repository.GetAllActiveAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<TramDienDto>> GetByKhuVucAsync(int idKhuVuc)
        {
            var items = await _repository.GetByKhuVucAsync(idKhuVuc);
            return items.Select(x => MapToDto(x));
        }

        public async Task<TramDienDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
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
            return MapToDto(entity);
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

        private static TramDienDto MapToDto(CBM_TramDien x) => new()
        {
            IDTram = x.IDTram,
            IDKhuVuc = x.IDKhuVuc,
            TenTram = x.TenTram,
            DiaDiem = x.DiaDiem,
            TrangThai = x.TrangThai
        };
    }
}
