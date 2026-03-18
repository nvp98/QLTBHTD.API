using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuService : IChiTieuService
    {
        private readonly IChiTieuRepository _repository;

        public ChiTieuService(IChiTieuRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ChiTieuDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<ChiTieuDto>> GetAllActiveAsync()
        {
            var items = await _repository.GetAllActiveAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<ChiTieuDto>> GetByNhomChiTieuAsync(int idNhomChiTieu)
        {
            var items = await _repository.GetByNhomChiTieuAsync(idNhomChiTieu);
            return items.Select(x => MapToDto(x));
        }

        public async Task<ChiTieuDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

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
            return MapToDto(entity);
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

        private static ChiTieuDto MapToDto(CBM_ChiTieu x) => new()
        {
            ID_ChiTieu = x.ID_ChiTieu,
            ID_NhomChiTieu = x.ID_NhomChiTieu,
            TenChiTieu = x.TenChiTieu,
            TrongSo_Wi = x.TrongSo_Wi,
            TrangThai = x.TrangThai
        };
    }
}
