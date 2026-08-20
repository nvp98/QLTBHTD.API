using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class ThongSoService : IThongSoService
    {
        private readonly IThongSoRepository _repo;
        private readonly IAppDbContext _db;

        public ThongSoService(IThongSoRepository repo, IAppDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        private static ThongSoDto ToDto(CBM_ThongSo e) => new()
        {
            ID_ThongSo = e.ID_ThongSo,
            MaThongSo  = e.MaThongSo,
            TenThongSo = e.TenThongSo,
            DonVi      = e.DonVi,
            LoaiDuLieu = e.LoaiDuLieu,
            TrangThai  = e.TrangThai,
            GhiChu     = e.GhiChu,
        };

        public async Task<List<ThongSoDto>> GetAllAsync()
            => (await _repo.GetAllAsync()).Select(ToDto).OrderBy(x => x.MaThongSo).ToList();

        public async Task<ThongSoDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : ToDto(entity);
        }

        public async Task<ThongSoDto> CreateAsync(CreateThongSoDto dto)
        {
            var entity = new CBM_ThongSo
            {
                MaThongSo  = dto.MaThongSo,
                TenThongSo = dto.TenThongSo,
                DonVi      = dto.DonVi,
                LoaiDuLieu = dto.LoaiDuLieu,
                TrangThai  = dto.TrangThai,
                GhiChu     = dto.GhiChu,
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<ThongSoDto?> UpdateAsync(int id, UpdateThongSoDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.MaThongSo  = dto.MaThongSo;
            entity.TenThongSo = dto.TenThongSo;
            entity.DonVi      = dto.DonVi;
            entity.LoaiDuLieu = dto.LoaiDuLieu;
            entity.TrangThai  = dto.TrangThai;
            entity.GhiChu     = dto.GhiChu;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return ToDto(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            var soThietBiDangDung = await _db.ThietBiThongSos.CountAsync(x => x.ID_ThongSo == id);
            if (soThietBiDangDung > 0)
                throw new ThongSoDangSuDungException(id, soThietBiDangDung);

            _repo.Delete(entity);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
