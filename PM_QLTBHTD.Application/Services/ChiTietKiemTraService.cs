using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTietKiemTraService : IChiTietKiemTraService
    {
        private readonly IChiTietKiemTraRepository _repository;

        public ChiTietKiemTraService(IChiTietKiemTraRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ChiTietKiemTraDto>> GetByPhieuAsync(int idPhieu)
        {
            var items = await _repository.GetByPhieuAsync(idPhieu);
            return items.Select(x => MapToDto(x));
        }

        public async Task<ChiTietKiemTraDto?> GetByIdAsync(int id)
        {
            var item = await _repository.GetByIdAsync(id);
            return item == null ? null : MapToDto(item);
        }

        public async Task<ChiTietKiemTraDto> CreateAsync(int idPhieu, CreateChiTietKiemTraDto dto)
        {
            var entity = new ChiTietKiemTra
            {
                IDPhieu = idPhieu,
                ID_ChiTieu = dto.ID_ChiTieu,
                GiaTriNhap_So = dto.GiaTriNhap_So,
                GiaTriNhap_Chu = dto.GiaTriNhap_Chu,
                GhiChu = dto.GhiChu
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<ChiTietKiemTraDto?> UpdateAsync(int id, UpdateChiTietKiemTraDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.IDPhieu = dto.IDPhieu;
            entity.ID_ChiTieu = dto.ID_ChiTieu;
            entity.GiaTriNhap_So = dto.GiaTriNhap_So;
            entity.GiaTriNhap_Chu = dto.GiaTriNhap_Chu;
            entity.Diem_Si_DatDuoc = dto.Diem_Si_DatDuoc;
            entity.GhiChu = dto.GhiChu;
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

        private static ChiTietKiemTraDto MapToDto(ChiTietKiemTra x) => new()
        {
            ID_ChiTiet = x.ID_ChiTiet,
            IDPhieu = x.IDPhieu,
            ID_ChiTieu = x.ID_ChiTieu,
            GiaTriNhap_So = x.GiaTriNhap_So,
            GiaTriNhap_Chu = x.GiaTriNhap_Chu,
            Diem_Si_DatDuoc = x.Diem_Si_DatDuoc,
            GhiChu = x.GhiChu
        };
    }
}
