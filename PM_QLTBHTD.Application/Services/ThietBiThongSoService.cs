using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ThietBiThongSoService : IThietBiThongSoService
    {
        private readonly IThietBiThongSoRepository _repo;
        private readonly IAppDbContext _db;

        public ThietBiThongSoService(IThietBiThongSoRepository repo, IAppDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        private async Task<List<ThietBiThongSoDto>> ToDtoListAsync(List<CBM_ThietBi_ThongSo> entities)
        {
            if (entities.Count == 0) return new List<ThietBiThongSoDto>();

            var idThongSos = entities.Select(x => x.ID_ThongSo).Distinct().ToList();
            var thongSos = await _db.ThongSos
                .Where(t => idThongSos.Contains(t.ID_ThongSo))
                .ToDictionaryAsync(t => t.ID_ThongSo);

            return entities.Select(e =>
            {
                thongSos.TryGetValue(e.ID_ThongSo, out var ts);
                return new ThietBiThongSoDto
                {
                    ID_ThietBi_ThongSo = e.ID_ThietBi_ThongSo,
                    ID_ThietBi = e.ID_ThietBi,
                    ID_ThongSo = e.ID_ThongSo,
                    MaThongSo  = ts?.MaThongSo ?? string.Empty,
                    TenThongSo = ts?.TenThongSo ?? string.Empty,
                    DonVi      = ts?.DonVi,
                    GiaTri     = e.GiaTri,
                    GhiChu     = e.GhiChu,
                };
            }).ToList();
        }

        private async Task<ThietBiThongSoDto> ToDtoAsync(CBM_ThietBi_ThongSo e)
            => (await ToDtoListAsync(new List<CBM_ThietBi_ThongSo> { e })).Single();

        public async Task<List<ThietBiThongSoDto>> GetByThietBiAsync(int idThietBi)
            => await ToDtoListAsync(await _repo.GetByThietBiAsync(idThietBi));

        public async Task<List<ThietBiThongSoUsageDto>> GetByThongSoAsync(int idThongSo)
        {
            var rows = await _repo.GetByThongSoAsync(idThongSo);
            if (rows.Count == 0) return new List<ThietBiThongSoUsageDto>();

            var idThietBis = rows.Select(x => x.ID_ThietBi).Distinct().ToList();
            var tenThietBis = await _db.ThietBis
                .Where(t => idThietBis.Contains(t.ID_ThietBi))
                .Select(t => new { t.ID_ThietBi, t.TenThietBi })
                .ToDictionaryAsync(t => t.ID_ThietBi, t => t.TenThietBi);

            return rows.Select(e => new ThietBiThongSoUsageDto
            {
                ID_ThietBi_ThongSo = e.ID_ThietBi_ThongSo,
                ID_ThietBi = e.ID_ThietBi,
                TenThietBi = tenThietBis.TryGetValue(e.ID_ThietBi, out var ten) ? ten : $"#{e.ID_ThietBi}",
                GiaTri = e.GiaTri,
                GhiChu = e.GhiChu,
            }).ToList();
        }

        public async Task<ThietBiThongSoDto> CreateAsync(CreateThietBiThongSoDto dto)
        {
            var entity = new CBM_ThietBi_ThongSo
            {
                ID_ThietBi = dto.ID_ThietBi,
                ID_ThongSo = dto.ID_ThongSo,
                GiaTri     = dto.GiaTri,
                GhiChu     = dto.GhiChu,
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return await ToDtoAsync(entity);
        }

        public async Task<ThietBiThongSoDto?> UpdateAsync(int id, UpdateThietBiThongSoDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_ThongSo = dto.ID_ThongSo;
            entity.GiaTri     = dto.GiaTri;
            entity.GhiChu     = dto.GhiChu;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return await ToDtoAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
