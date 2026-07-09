using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;

namespace PM_QLTBHTD.Application.Services
{
    public class ThongKeService : IThongKeService
    {
        private readonly IAppDbContext _db;

        public ThongKeService(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<ThongKeTongHopDto> GetTongHopAsync()
        {
            var tongThietBi = await _db.ThietBis.CountAsync(t => t.TrangThai == 1);

            // ID_Phieu moi nhat cua moi thiet bi (ID lon nhat = moi nhat)
            var latestIds = await _db.PhieuKiemTras
                .GroupBy(p => p.ID_ThietBi)
                .Select(g => g.Max(x => x.ID_Phieu))
                .ToListAsync();

            var latestPhieus = await _db.PhieuKiemTras
                .Where(p => latestIds.Contains(p.ID_Phieu))
                .ToListAsync();

            int tot = 0, binhThuong = 0, chuY = 0, canhBao = 0, nguHiem = 0;
            foreach (var p in latestPhieus)
            {
                if (p.TongDiem_Soqt == null) continue;
                var d = (double)p.TongDiem_Soqt.Value;
                if      (d >= 8) tot++;
                else if (d >= 6) binhThuong++;
                else if (d >= 4) chuY++;
                else if (d >= 2) canhBao++;
                else             nguHiem++;
            }

            var chuaKiemTra = tongThietBi - latestPhieus.Count;

            var thangDau = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var tongPhieuThang = await _db.PhieuKiemTras.CountAsync(p => p.NgayKiemTra >= thangDau);

            var validDiems = latestPhieus
                .Where(p => p.TongDiem_Soqt.HasValue)
                .Select(p => (double)p.TongDiem_Soqt!.Value)
                .ToList();

            double? diemTB = validDiems.Count > 0 ? validDiems.Average() : null;

            return new ThongKeTongHopDto
            {
                TongThietBi        = tongThietBi,
                ThietBiTot         = tot,
                ThietBiBinhThuong  = binhThuong,
                ThietBiChuY        = chuY,
                ThietBiCanhBao     = canhBao,
                ThietBiNguyHiem    = nguHiem,
                ThietBiChuaKiemTra = chuaKiemTra < 0 ? 0 : chuaKiemTra,
                TongPhieuThangNay  = tongPhieuThang,
                DiemTrungBinh      = diemTB,
            };
        }

        public async Task<IEnumerable<LichSuCSSKDto>> GetLichSuThietBiAsync(int idThietBi)
        {
            return await _db.PhieuKiemTras
                .Where(p => p.ID_ThietBi == idThietBi)
                .OrderBy(p => p.NgayKiemTra)
                .Select(p => new LichSuCSSKDto
                {
                    ID_Phieu      = p.ID_Phieu,
                    NgayKiemTra   = p.NgayKiemTra,
                    TongDiem_Soqt = p.TongDiem_Soqt,
                    CapDoCanhBao  = p.CapDoCanhBao,
                    NguoiKiemTra  = p.NguoiKiemTra,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<BaoCaoTramItemDto>> GetBaoCaoTramAsync()
        {
            var trams = await _db.TramDiens.ToListAsync();

            var thietBiList = await _db.ThietBis
                .Where(t => t.TrangThai == 1)
                .Select(t => new { t.ID_ThietBi, t.ID_Tram })
                .ToListAsync();

            // Lay ID phieu moi nhat moi thiet bi
            var latestIds = await _db.PhieuKiemTras
                .GroupBy(p => p.ID_ThietBi)
                .Select(g => new { ID_ThietBi = g.Key, MaxId = g.Max(x => x.ID_Phieu) })
                .ToListAsync();

            var latestPhieus = await _db.PhieuKiemTras
                .Where(p => latestIds.Select(x => x.MaxId).Contains(p.ID_Phieu))
                .Select(p => new { p.ID_ThietBi, p.TongDiem_Soqt })
                .ToListAsync();

            var latestMap = latestPhieus.ToDictionary(x => x.ID_ThietBi, x => x.TongDiem_Soqt);

            var result = new List<BaoCaoTramItemDto>();
            foreach (var tram in trams.OrderBy(t => t.TenTram))
            {
                var tbInTram = thietBiList.Where(t => t.ID_Tram == tram.IDTram).ToList();
                var diems = tbInTram
                    .Where(t => latestMap.TryGetValue(t.ID_ThietBi, out var d) && d.HasValue)
                    .Select(t => (double)latestMap[t.ID_ThietBi]!.Value)
                    .ToList();

                int tot = 0, binh = 0, chuY = 0, canhBao = 0, nguHiem = 0;
                foreach (var d in diems)
                {
                    if      (d >= 8) tot++;
                    else if (d >= 6) binh++;
                    else if (d >= 4) chuY++;
                    else if (d >= 2) canhBao++;
                    else             nguHiem++;
                }

                result.Add(new BaoCaoTramItemDto
                {
                    IDTram          = tram.IDTram,
                    TenTram         = tram.TenTram,
                    DiaDiem         = tram.DiaDiem,
                    TongThietBi     = tbInTram.Count,
                    DaKiemTra       = diems.Count,
                    DiemTrungBinh   = diems.Count > 0 ? diems.Average() : null,
                    TotCount        = tot,
                    BinhThuongCount = binh,
                    ChuYCount       = chuY,
                    CanhBaoCount    = canhBao,
                    NguHiemCount    = nguHiem,
                });
            }

            return result;
        }

        public async Task<IEnumerable<CanhBaoThietBiDto>> GetCanhBaoAsync()
        {
            // Lay tat ca phieu co CSSK < 6 (thang 0-10)
            var rows = await (
                from p in _db.PhieuKiemTras
                join tb   in _db.ThietBis      on p.ID_ThietBi    equals tb.ID_ThietBi
                join tram in _db.TramDiens     on tb.ID_Tram       equals tram.IDTram
                join loai in _db.LoaiThietBis  on tb.ID_LoaiTB     equals loai.ID_LoaiThietBi
                where p.TongDiem_Soqt != null && p.TongDiem_Soqt < 6
                select new
                {
                    p.ID_ThietBi,
                    tb.TenThietBi,
                    tram.TenTram,
                    loai.KyHieu,
                    p.ID_Phieu,
                    p.NgayKiemTra,
                    p.TongDiem_Soqt,
                    p.CapDoCanhBao,
                }
            ).ToListAsync();

            // Chi giu phieu moi nhat moi thiet bi
            return rows
                .GroupBy(r => r.ID_ThietBi)
                .Select(g => g.OrderByDescending(r => r.NgayKiemTra).First())
                .OrderBy(r => r.TongDiem_Soqt)
                .Select(r => new CanhBaoThietBiDto
                {
                    ID_ThietBi    = r.ID_ThietBi,
                    TenThietBi    = r.TenThietBi,
                    TenTram       = r.TenTram,
                    KyHieu        = r.KyHieu,
                    ID_Phieu      = r.ID_Phieu,
                    NgayKiemTra   = r.NgayKiemTra,
                    TongDiem_Soqt = r.TongDiem_Soqt,
                    CapDoCanhBao  = r.CapDoCanhBao
                        ?? ((double?)r.TongDiem_Soqt >= 4 ? "Chu y"
                           : (double?)r.TongDiem_Soqt >= 2 ? "Canh bao"
                           : "Nguy hiem"),
                });
        }
    }
}
