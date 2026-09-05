using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;

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

        /// <summary>CSSK trung bình + phân bố hạng theo TỪNG LOẠI THIẾT BỊ (MBA, MC, DCL...) — thay
        /// cho 1 số trung bình gộp toàn hệ thống vốn không có ý nghĩa vì mỗi loại thiết bị dùng bộ
        /// chỉ tiêu/công thức tính điểm khác nhau và nằm ở các trạm độc lập với nhau.</summary>
        public async Task<IEnumerable<TongHopTheoLoaiDto>> GetTongHopTheoLoaiAsync()
        {
            var loais = await _db.LoaiThietBis.ToListAsync();

            var thietBiList = await _db.ThietBis
                .Where(t => t.TrangThai == 1)
                .Select(t => new { t.ID_ThietBi, t.ID_LoaiTB })
                .ToListAsync();

            var latestIds = await _db.PhieuKiemTras
                .GroupBy(p => p.ID_ThietBi)
                .Select(g => g.Max(x => x.ID_Phieu))
                .ToListAsync();

            var latestPhieus = await _db.PhieuKiemTras
                .Where(p => latestIds.Contains(p.ID_Phieu))
                .Select(p => new { p.ID_ThietBi, p.TongDiem_Soqt })
                .ToListAsync();

            var latestMap = latestPhieus.ToDictionary(x => x.ID_ThietBi, x => x.TongDiem_Soqt);

            var result = new List<TongHopTheoLoaiDto>();
            foreach (var loai in loais.OrderBy(l => l.TenLoaiTB))
            {
                var tbInLoai = thietBiList.Where(t => t.ID_LoaiTB == loai.ID_LoaiThietBi).ToList();
                if (tbInLoai.Count == 0) continue; // bỏ qua loại chưa có thiết bị nào đang hoạt động

                var diems = tbInLoai
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

                result.Add(new TongHopTheoLoaiDto
                {
                    ID_LoaiTB       = loai.ID_LoaiThietBi,
                    TenLoaiTB       = loai.TenLoaiTB,
                    KyHieu          = loai.KyHieu,
                    TongThietBi     = tbInLoai.Count,
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

        /// <summary>
        /// Cảnh báo thiết bị mức 0-10 &lt; 6, xét PHIẾU MỚI NHẤT của mỗi thiết bị. Thiết bị có CSSK
        /// tổng (TongDiem_Soqt) dùng luôn giá trị đó — như trước. Thiết bị KHÔNG có CSSK tổng (loại
        /// thiết bị theo quy trình không tính CHI cấp 1/2/3, vd DCL/TU/TI/CS — chỉ nhập liệu + cảnh
        /// báo theo từng chỉ tiêu) trước đây bị bỏ sót hoàn toàn khỏi danh sách này vì lọc thẳng
        /// theo TongDiem_Soqt — giờ fallback lấy Sᵢ THẤP NHẤT trong các chỉ tiêu đã đo của phiếu đó
        /// làm đại diện mức cảnh báo, để nhóm thiết bị này cũng lên được dashboard.
        /// </summary>
        public async Task<IEnumerable<CanhBaoThietBiDto>> GetCanhBaoAsync()
        {
            // Phiếu mới nhất của mỗi thiết bị — KHÔNG lọc theo TongDiem_Soqt ở bước này vì cần xét
            // cả trường hợp null (fallback qua Sᵢ chỉ tiêu).
            var latestIds = await _db.PhieuKiemTras
                .GroupBy(p => p.ID_ThietBi)
                .Select(g => g.Max(x => x.ID_Phieu))
                .ToListAsync();

            var phieuInfo = await (
                from p in _db.PhieuKiemTras
                join tb   in _db.ThietBis      on p.ID_ThietBi    equals tb.ID_ThietBi
                join tram in _db.TramDiens     on tb.ID_Tram       equals tram.IDTram
                join loai in _db.LoaiThietBis  on tb.ID_LoaiTB     equals loai.ID_LoaiThietBi
                where latestIds.Contains(p.ID_Phieu)
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

            // Chỉ tiêu có Sᵢ THẤP NHẤT của mỗi phiếu — dùng để: (1) fallback DiemHienThi cho thiết bị
            // không có CSSK tổng, và (2) gợi ý ngay "cần làm gì" (khuyến cáo hành động) cho MỌI thiết
            // bị cảnh báo, kể cả thiết bị đã có CSSK tổng.
            var allPhieuIds = phieuInfo.Select(x => x.ID_Phieu).ToList();
            Dictionary<int, (decimal Si, string TenChiTieu, string? KhuyenCao)> chiTieuThapNhatMap = allPhieuIds.Count == 0
                ? new Dictionary<int, (decimal Si, string TenChiTieu, string? KhuyenCao)>()
                : (await (
                    from ct in _db.ChiTietKiemTras
                    join c in _db.ChiTieus on ct.ID_ChiTieu equals c.ID_ChiTieu
                    where allPhieuIds.Contains(ct.IDPhieu) && ct.Diem_Si_DatDuoc != null
                    select new { ct.IDPhieu, Si = ct.Diem_Si_DatDuoc!.Value, c.TenChiTieu, ct.HanhDongKhuyenCao }
                ).ToListAsync())
                .GroupBy(x => x.IDPhieu)
                .ToDictionary(g => g.Key, g =>
                {
                    var thapNhat = g.OrderBy(x => x.Si).First();
                    return (thapNhat.Si, thapNhat.TenChiTieu, thapNhat.HanhDongKhuyenCao);
                });

            var result = new List<CanhBaoThietBiDto>();
            foreach (var p in phieuInfo)
            {
                decimal diemHienThi;
                string nguonDiem;
                string? tenChiTieuThapNhat = null;
                chiTieuThapNhatMap.TryGetValue(p.ID_Phieu, out var chiTieuThapNhat);

                if (p.TongDiem_Soqt != null)
                {
                    // CSSK tổng có thể vẫn qua ngưỡng "tốt" dù có 1 chỉ tiêu riêng lẻ rất kém — trọng
                    // số Wᵢ nhỏ của chỉ tiêu đó "trung hòa" mất trong điểm tổng. Nếu chỉ tiêu thấp nhất
                    // đã ở mức Cảnh báo/Nguy hiểm (Sᵢ < 4) mà điểm tổng lại ≥ 6, ưu tiên cảnh báo theo
                    // đúng chỉ tiêu đó thay vì im lặng bỏ qua chỉ vì điểm tổng qua ngưỡng.
                    if (chiTieuThapNhat.TenChiTieu != null && chiTieuThapNhat.Si < 4 && p.TongDiem_Soqt.Value >= 6)
                    {
                        diemHienThi = chiTieuThapNhat.Si;
                        nguonDiem = "CHI_TIEU_RIENG";
                        tenChiTieuThapNhat = chiTieuThapNhat.TenChiTieu;
                    }
                    else
                    {
                        diemHienThi = p.TongDiem_Soqt.Value;
                        nguonDiem = "CSSK";
                    }
                }
                else if (chiTieuThapNhat.TenChiTieu != null)
                {
                    diemHienThi = chiTieuThapNhat.Si;
                    nguonDiem = "CHI_TIEU";
                    tenChiTieuThapNhat = chiTieuThapNhat.TenChiTieu;
                }
                else
                {
                    continue; // chưa đo chỉ tiêu nào cả — không đủ dữ liệu để đánh giá
                }

                if (diemHienThi >= 6) continue;

                result.Add(new CanhBaoThietBiDto
                {
                    ID_ThietBi          = p.ID_ThietBi,
                    TenThietBi          = p.TenThietBi,
                    TenTram             = p.TenTram,
                    KyHieu              = p.KyHieu,
                    ID_Phieu            = p.ID_Phieu,
                    NgayKiemTra         = p.NgayKiemTra,
                    TongDiem_Soqt       = p.TongDiem_Soqt,
                    DiemHienThi         = diemHienThi,
                    NguonDiem           = nguonDiem,
                    TenChiTieuThapNhat  = tenChiTieuThapNhat,
                    KhuyenCaoHanhDong   = chiTieuThapNhat.KhuyenCao,
                    // CapDoCanhBao đã lưu sẵn trên phiếu (p.CapDoCanhBao) được tính theo CSSK TỔNG —
                    // chỉ dùng khi NguonDiem="CSSK"; 2 trường hợp còn lại (CHI_TIEU/CHI_TIEU_RIENG)
                    // phải tính lại theo đúng DiemHienThi (điểm chỉ tiêu), không thì hiển thị sai mức.
                    CapDoCanhBao        = nguonDiem == "CSSK" && p.CapDoCanhBao != null
                        ? p.CapDoCanhBao
                        : ((double)diemHienThi >= 4 ? "Chu y"
                           : (double)diemHienThi >= 2 ? "Canh bao"
                           : "Nguy hiem"),
                });
            }

            return result.OrderBy(r => r.DiemHienThi);
        }

        /// <summary>Xu hướng CSSK trung bình toàn hệ thống theo tháng, tính trên TẤT CẢ phiếu kiểm tra
        /// có CSSK tổng trong tháng đó (không chỉ phiếu mới nhất) — phản ánh chất lượng thiết bị được
        /// kiểm tra trong từng tháng, giúp giám đốc trung tâm giám sát thấy xu hướng tăng/giảm.</summary>
        public async Task<IEnumerable<XuHuongThangDto>> GetXuHuongThangAsync(int soThang, int? idTram, int? idLoaiTB, int? idThietBi)
        {
            if (soThang <= 0) soThang = 6;
            var tuThang = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-(soThang - 1));

            var query =
                from p in _db.PhieuKiemTras
                join tb in _db.ThietBis on p.ID_ThietBi equals tb.ID_ThietBi
                where p.NgayKiemTra >= tuThang
                   && (idTram == null || tb.ID_Tram == idTram)
                   && (idLoaiTB == null || tb.ID_LoaiTB == idLoaiTB)
                   && (idThietBi == null || tb.ID_ThietBi == idThietBi)
                select new { p.NgayKiemTra, p.TongDiem_Soqt };

            var phieus = await query.ToListAsync();

            var byThang = phieus
                .GroupBy(p => new DateTime(p.NgayKiemTra.Year, p.NgayKiemTra.Month, 1))
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<XuHuongThangDto>();
            for (var i = 0; i < soThang; i++)
            {
                var thang = tuThang.AddMonths(i);
                byThang.TryGetValue(thang, out var items);
                items ??= new();
                var diems = items.Where(x => x.TongDiem_Soqt.HasValue).Select(x => (double)x.TongDiem_Soqt!.Value).ToList();

                result.Add(new XuHuongThangDto
                {
                    Thang         = thang.ToString("yyyy-MM"),
                    DiemTrungBinh = diems.Count > 0 ? diems.Average() : null,
                    SoPhieu       = items.Count,
                });
            }

            return result;
        }
    }
}
