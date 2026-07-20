using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class CongThucTongHopService : ICongThucTongHopService
    {
        private readonly ICongThucTongHopRepository _repo;
        private readonly IAppDbContext _db;

        public CongThucTongHopService(ICongThucTongHopRepository repo, IAppDbContext db)
        {
            _repo = repo;
            _db = db;
        }

        private IQueryable<CongThucTongHopDto> JoinQuery()
        {
            return from c in _db.CongThucTongHops
                   join n in _db.NhomChiTieus on c.ID_NhomChiTieu equals n.ID_NhomChiTieu
                   select new CongThucTongHopDto
                   {
                       ID_CongThuc = c.ID_CongThuc,
                       ID_NhomChiTieu = c.ID_NhomChiTieu,
                       TenNhom = n.TenNhom,
                       BieuThuc = c.BieuThuc,
                       LoaiCongThuc = c.LoaiCongThuc,
                       PhienBan = c.PhienBan,
                       TrangThai = c.TrangThai,
                       ThangDiem_Min = c.ThangDiem_Min,
                       ThangDiem_Max = c.ThangDiem_Max,
                       MoTa = c.MoTa
                   };
        }

        public async Task<IEnumerable<CongThucTongHopDto>> GetByNhomAsync(int idNhomChiTieu)
        {
            var list = await JoinQuery().Where(x => x.ID_NhomChiTieu == idNhomChiTieu).ToListAsync();
            foreach (var item in list)
                item.DanhSachBien = await GetBienAsync(item.ID_CongThuc);
            return list;
        }

        public async Task<CongThucTongHopDto?> GetActiveByNhomAsync(int idNhomChiTieu)
        {
            var item = await JoinQuery()
                .FirstOrDefaultAsync(x => x.ID_NhomChiTieu == idNhomChiTieu && x.TrangThai == 1);
            if (item != null)
                item.DanhSachBien = await GetBienAsync(item.ID_CongThuc);
            return item;
        }

        public async Task<CongThucTongHopDto?> GetByIdAsync(int id)
        {
            var item = await JoinQuery().FirstOrDefaultAsync(x => x.ID_CongThuc == id);
            if (item != null)
                item.DanhSachBien = await GetBienAsync(item.ID_CongThuc);
            return item;
        }

        public async Task<CongThucTongHopDto> CreateAsync(CreateCongThucTongHopDto dto)
        {
            // Deactivate công thức cũ trước khi tạo mới
            var cuList = await _repo.GetByNhomAsync(dto.ID_NhomChiTieu);
            foreach (var cu in cuList.Where(x => x.TrangThai == 1))
            {
                cu.TrangThai = 0;
                _repo.Update(cu);
            }

            var entity = new CBM_CongThucTongHop
            {
                ID_NhomChiTieu = dto.ID_NhomChiTieu,
                BieuThuc = dto.BieuThuc,
                LoaiCongThuc = dto.LoaiCongThuc,
                PhienBan = dto.PhienBan,
                TrangThai = 1,
                ThangDiem_Min = dto.ThangDiem_Min,
                ThangDiem_Max = dto.ThangDiem_Max,
                MoTa = dto.MoTa
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return (await GetByIdAsync(entity.ID_CongThuc))!;
        }

        public async Task<CongThucTongHopDto?> UpdateAsync(int id, UpdateCongThucTongHopDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.BieuThuc = dto.BieuThuc;
            entity.LoaiCongThuc = dto.LoaiCongThuc;
            entity.TrangThai = dto.TrangThai;
            entity.ThangDiem_Min = dto.ThangDiem_Min;
            entity.ThangDiem_Max = dto.ThangDiem_Max;
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

        private async Task<List<CongThucBienDto>> GetBienAsync(int idCongThuc)
        {
            return await (from b in _db.CongThucBiens
                          where b.ID_CongThuc == idCongThuc
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
                          }).ToListAsync();
        }
    }
}
