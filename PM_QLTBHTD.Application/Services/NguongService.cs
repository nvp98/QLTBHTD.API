using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class NguongService : INguongService
    {
        private readonly INguongRepository _repository;

        public NguongService(INguongRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<NguongDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<NguongDto>> GetByChiTieuAsync(int idChiTieu)
        {
            var items = await _repository.GetByChiTieuAsync(idChiTieu);
            return items.Select(x => MapToDto(x));
        }

        public async Task<NguongDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

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
            return MapToDto(entity);
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

        private static NguongDto MapToDto(CBM_Nguong x) => new()
        {
            ID_Nguong = x.ID_Nguong,
            ID_ChiTieu = x.ID_ChiTieu,
            CanTren = x.CanTren,
            CanDuoi = x.CanDuoi,
            Diem_Si = x.Diem_Si
        };
    }
}
