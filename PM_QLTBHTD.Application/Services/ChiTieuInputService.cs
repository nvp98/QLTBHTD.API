using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuInputService : IChiTieuInputService
    {
        private readonly IChiTieuInputRepository _repository;

        public ChiTieuInputService(IChiTieuInputRepository repository)
            => _repository = repository;

        private static ChiTieuInputDto ToDto(CBM_ChiTieu_Input e) => new()
        {
            ID_Input   = e.ID_Input,
            ID_ChiTieu = e.ID_ChiTieu,
            MaInput    = e.MaInput,
            TenInput   = e.TenInput,
        };

        public async Task<IEnumerable<ChiTieuInputDto>> GetByChiTieuAsync(int idChiTieu)
            => (await _repository.GetByChiTieuAsync(idChiTieu)).Select(ToDto);

        public async Task<ChiTieuInputDto?> GetByIdAsync(int id)
        {
            var e = await _repository.GetByIdAsync(id);
            return e == null ? null : ToDto(e);
        }

        public async Task<ChiTieuInputDto> CreateAsync(CreateChiTieuInputDto dto)
        {
            var entity = new CBM_ChiTieu_Input
            {
                ID_ChiTieu = dto.ID_ChiTieu,
                MaInput    = dto.MaInput,
                TenInput   = dto.TenInput,
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<ChiTieuInputDto?> UpdateAsync(int id, UpdateChiTieuInputDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_ChiTieu = dto.ID_ChiTieu;
            entity.MaInput    = dto.MaInput;
            entity.TenInput   = dto.TenInput;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return ToDto(entity);
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
