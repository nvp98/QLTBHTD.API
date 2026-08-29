using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.Application.Services
{
    public class LichSuService : ILichSuService
    {
        private readonly IAppDbContext _db;

        public LichSuService(IAppDbContext db) => _db = db;

        public async Task<LichSuNhomDto?> GetLichSuAsync(int idThietBi, int idNhomChiTieu)
        {
            var thietBi = await _db.ThietBis.FirstOrDefaultAsync(t => t.ID_ThietBi == idThietBi);
            if (thietBi == null) return null;

            var nhom = await _db.NhomChiTieus.FirstOrDefaultAsync(n => n.ID_NhomChiTieu == idNhomChiTieu);
            if (nhom == null) return null;

            var chiTieus = await _db.ChiTieus
                .Where(c => c.ID_NhomChiTieu == idNhomChiTieu && c.TrangThai == 1)
                .Select(c => new LichSuChiTieuColDto { ID_ChiTieu = c.ID_ChiTieu, TenChiTieu = c.TenChiTieu ?? "" })
                .ToListAsync();

            var idChiTieus = chiTieus.Select(c => c.ID_ChiTieu).ToList();

            var chiTiet = await (
                from ct in _db.ChiTietKiemTras
                join p in _db.PhieuKiemTras on ct.IDPhieu equals p.ID_Phieu
                where p.ID_ThietBi == idThietBi && idChiTieus.Contains(ct.ID_ChiTieu)
                select new
                {
                    p.ID_Phieu,
                    p.NgayKiemTra,
                    p.SoPhieu,
                    ct.ID_ChiTieu,
                    ct.GiaTriNhap_So,
                    ct.Diem_Si_DatDuoc,
                }
            ).ToListAsync();

            var idPhieus = chiTiet.Select(x => x.ID_Phieu).Distinct().ToList();

            var diemNhomTheoPhieu = await _db.KetQuaNhoms
                .Where(k => k.ID_NhomChiTieu == idNhomChiTieu && idPhieus.Contains(k.IDPhieu))
                .ToDictionaryAsync(k => k.IDPhieu, k => k.Diem);

            var hang = chiTiet
                .GroupBy(x => new { x.ID_Phieu, x.NgayKiemTra, x.SoPhieu })
                .Select(g => new LichSuHangDto
                {
                    ID_Phieu = g.Key.ID_Phieu,
                    NgayKiemTra = g.Key.NgayKiemTra,
                    SoPhieu = g.Key.SoPhieu,
                    DiemNhom = diemNhomTheoPhieu.TryGetValue(g.Key.ID_Phieu, out var d) ? d : null,
                    GiaTriTheoChiTieu = g.ToDictionary(
                        x => x.ID_ChiTieu,
                        x => new LichSuGiaTriDto { GiaTri = x.GiaTriNhap_So, Si = x.Diem_Si_DatDuoc }),
                })
                .OrderBy(h => h.NgayKiemTra)
                .ToList();

            return new LichSuNhomDto
            {
                ID_ThietBi = idThietBi,
                TenThietBi = thietBi.TenThietBi ?? "",
                ID_NhomChiTieu = idNhomChiTieu,
                TenNhom = nhom.TenNhom ?? "",
                ChiTieus = chiTieus,
                Hang = hang,
            };
        }
    }
}
