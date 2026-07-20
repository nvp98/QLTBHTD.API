using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class CongThucBienService : ICongThucBienService
    {
        private readonly ICongThucBienRepository _repo;
        private readonly IAppDbContext _db;

        public CongThucBienService(ICongThucBienRepository repo, IAppDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        private IQueryable<CongThucBienDto> JoinQuery()
        {
            return from b in _db.CongThucBiens
                   join ct in _db.ChiTieus on b.ID_ChiTieuNguon equals ct.ID_ChiTieu into ctGroup
                   from ct in ctGroup.DefaultIfEmpty()
                   join nh in _db.NhomChiTieus on b.ID_NhomCon equals nh.ID_NhomChiTieu into nhGroup
                   from nh in nhGroup.DefaultIfEmpty()
                   select new CongThucBienDto
                   {
                       ID_Bien = b.ID_Bien,
                       ID_CongThuc = b.ID_CongThuc,
                       MaBien = b.MaBien,
                       NguonBien = b.NguonBien,
                       ID_ChiTieuNguon = b.ID_ChiTieuNguon,
                       TenChiTieu = ct != null ? ct.TenChiTieu : null,
                       ID_NhomCon = b.ID_NhomCon,
                       TenNhomCon = nh != null ? nh.TenNhom : null,
                       GiaTriHangSo = b.GiaTriHangSo,
                       TrongSo = b.TrongSo,
                       MoTa = b.MoTa
                   };
        }

        public async Task<IEnumerable<CongThucBienDto>> GetByCongThucAsync(int idCongThuc)
            => await JoinQuery().Where(x => x.ID_CongThuc == idCongThuc).ToListAsync();

        public async Task<CongThucBienDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_Bien == id);

        public async Task<CongThucBienDto> CreateAsync(CreateCongThucBienDto dto)
        {
            var entity = new CBM_CongThuc_Bien
            {
                ID_CongThuc = dto.ID_CongThuc,
                MaBien = dto.MaBien,
                NguonBien = dto.NguonBien,
                ID_ChiTieuNguon = dto.ID_ChiTieuNguon,
                ID_NhomCon = dto.ID_NhomCon,
                GiaTriHangSo = dto.GiaTriHangSo,
                TrongSo = dto.TrongSo,
                MoTa = dto.MoTa
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return (await GetByIdAsync(entity.ID_Bien))!;
        }

        public async Task<CongThucBienDto?> UpdateAsync(int id, UpdateCongThucBienDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.MaBien = dto.MaBien;
            entity.NguonBien = dto.NguonBien;
            entity.ID_ChiTieuNguon = dto.ID_ChiTieuNguon;
            entity.ID_NhomCon = dto.ID_NhomCon;
            entity.GiaTriHangSo = dto.GiaTriHangSo;
            entity.TrongSo = dto.TrongSo;
            entity.MoTa = dto.MoTa;
            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return await GetByIdAsync(id);
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
