using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ChiTieuInputService : IChiTieuInputService
    {
        private readonly IChiTieuInputRepository _repository;
        private readonly IAppDbContext _db;

        public ChiTieuInputService(IChiTieuInputRepository repository, IAppDbContext db)
        {
            _repository = repository;
            _db = db;
        }

        private static ChiTieuInputDto ToDto(CBM_ChiTieu_Input e, string? tenChiTieuNguon) => new()
        {
            ID_Input   = e.ID_Input,
            ID_ChiTieu = e.ID_ChiTieu,
            MaInput    = e.MaInput,
            TenInput   = e.TenInput,
            NguonGiaTri     = e.NguonGiaTri ?? "MANUAL",
            ID_ChiTieuNguon = e.ID_ChiTieuNguon,
            TenChiTieuNguon = tenChiTieuNguon,
            MaThongSoThietBi = e.MaThongSoThietBi,
        };

        private async Task<ChiTieuInputDto> ToDtoAsync(CBM_ChiTieu_Input e)
        {
            string? tenChiTieuNguon = e.ID_ChiTieuNguon == null ? null : await _db.ChiTieus
                .Where(c => c.ID_ChiTieu == e.ID_ChiTieuNguon)
                .Select(c => c.TenChiTieu)
                .FirstOrDefaultAsync();
            return ToDto(e, tenChiTieuNguon);
        }

        public async Task<IEnumerable<ChiTieuInputDto>> GetByChiTieuAsync(int idChiTieu)
        {
            // KHÔNG dùng Task.WhenAll trên nhiều truy vấn EF Core cùng lúc — DbContext không an
            // toàn cho concurrent access, gây lỗi 500 "A second operation was started on this
            // context before a previous operation completed." Gom 1 truy vấn duy nhất thay vì
            // N truy vấn (N+1) cho từng dòng.
            var items = (await _repository.GetByChiTieuAsync(idChiTieu)).ToList();

            var idNguon = items.Where(x => x.ID_ChiTieuNguon != null)
                .Select(x => x.ID_ChiTieuNguon!.Value).Distinct().ToList();

            var tenTheoId = idNguon.Count == 0
                ? new Dictionary<int, string?>()
                : await _db.ChiTieus.Where(c => idNguon.Contains(c.ID_ChiTieu))
                    .ToDictionaryAsync(c => c.ID_ChiTieu, c => c.TenChiTieu);

            return items.Select(e => ToDto(e,
                e.ID_ChiTieuNguon != null && tenTheoId.TryGetValue(e.ID_ChiTieuNguon.Value, out var ten) ? ten : null));
        }

        public async Task<ChiTieuInputDto?> GetByIdAsync(int id)
        {
            var e = await _repository.GetByIdAsync(id);
            return e == null ? null : await ToDtoAsync(e);
        }

        public async Task<ChiTieuInputDto> CreateAsync(CreateChiTieuInputDto dto)
        {
            var entity = new CBM_ChiTieu_Input
            {
                ID_ChiTieu = dto.ID_ChiTieu,
                MaInput    = dto.MaInput,
                TenInput   = dto.TenInput,
                NguonGiaTri     = dto.NguonGiaTri,
                ID_ChiTieuNguon = dto.NguonGiaTri == "CHITIEU_CUNG_PHIEU" ? dto.ID_ChiTieuNguon : null,
                MaThongSoThietBi = dto.NguonGiaTri == "THIETBI_THONGSO" ? dto.MaThongSoThietBi : null,
            };
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return await ToDtoAsync(entity);
        }

        public async Task<ChiTieuInputDto?> UpdateAsync(int id, UpdateChiTieuInputDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_ChiTieu = dto.ID_ChiTieu;
            entity.MaInput    = dto.MaInput;
            entity.TenInput   = dto.TenInput;
            entity.NguonGiaTri     = dto.NguonGiaTri;
            entity.ID_ChiTieuNguon = dto.NguonGiaTri == "CHITIEU_CUNG_PHIEU" ? dto.ID_ChiTieuNguon : null;
            entity.MaThongSoThietBi = dto.NguonGiaTri == "THIETBI_THONGSO" ? dto.MaThongSoThietBi : null;

            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return await ToDtoAsync(entity);
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
