using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class NhomChiTieuService : INhomChiTieuService
    {
        private readonly INhomChiTieuRepository _repository;

        public NhomChiTieuService(INhomChiTieuRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<NhomChiTieuDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<NhomChiTieuDto>> GetAllActiveAsync()
        {
            var items = await _repository.GetAllActiveAsync();
            return items.Select(x => MapToDto(x));
        }

        public async Task<IEnumerable<NhomChiTieuDto>> GetByLoaiThietBiAsync(int idLoaiThietBi)
        {
            var items = await _repository.GetByLoaiThietBiAsync(idLoaiThietBi);
            return items.Select(x => MapToDto(x));
        }

        public async Task<NhomChiTieuDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

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
            return MapToDto(entity);
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

        private static NhomChiTieuDto MapToDto(CBM_NhomChiTieu x) => new()
        {
            ID_NhomChiTieu = x.ID_NhomChiTieu,
            TenNhom = x.TenNhom,
            ID_LoaiThietBi = x.ID_LoaiThietBi,
            PhienBan = x.PhienBan,
            TrangThai = x.TrangThai
        };
    }
}
