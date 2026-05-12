using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Helpers;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class PhieuKiemTraService : IPhieuKiemTraService
    {
        private readonly IPhieuKiemTraRepository _phieuRepo;
        private readonly IChiTietKiemTraRepository _chiTietRepo;
        private readonly IChiTietKiemTraInputRepository _chiTietInputRepo;
        private readonly INguongRepository _nguongRepo;
        private readonly IAppDbContext _db;

        public PhieuKiemTraService(
            IPhieuKiemTraRepository phieuRepo,
            IChiTietKiemTraRepository chiTietRepo,
            IChiTietKiemTraInputRepository chiTietInputRepo,
            INguongRepository nguongRepo,
            IAppDbContext db)
        {
            _phieuRepo = phieuRepo;
            _chiTietRepo = chiTietRepo;
            _chiTietInputRepo = chiTietInputRepo;
            _nguongRepo = nguongRepo;
            _db = db;
        }

        private IQueryable<PhieuKiemTraDto> JoinQuery()
        {
            return from p in _db.PhieuKiemTras
                   join tb in _db.ThietBis on p.ID_ThietBi equals tb.ID_ThietBi
                   join nhom in _db.NhomChiTieus on p.ID_NhomChiTieu equals nhom.ID_NhomChiTieu into nhomJoin
                   from nhom in nhomJoin.DefaultIfEmpty()
                   select new PhieuKiemTraDto
                   {
                       ID_Phieu       = p.ID_Phieu,
                       ID_ThietBi     = p.ID_ThietBi,
                       TenThietBi     = tb.TenThietBi,
                       ID_NhomChiTieu = p.ID_NhomChiTieu,
                       TenNhom        = nhom != null ? nhom.TenNhom : null,
                       NgayKiemTra    = p.NgayKiemTra,
                       NguoiKiemTra   = p.NguoiKiemTra,
                       TongDiem_Soqt  = p.TongDiem_Soqt,
                       CapDoCanhBao   = p.CapDoCanhBao,
                       GhiChuChung    = p.GhiChuChung
                   };
        }

        public async Task<PagedResult<PhieuKiemTraDto>> GetPagedAsync(string? search, int page, int pageSize)
        {
            var query = JoinQuery().Where(x =>
                string.IsNullOrEmpty(search)
                || x.TenThietBi.Contains(search)
                || (x.NguoiKiemTra != null && x.NguoiKiemTra.Contains(search)));

            var total = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.NgayKiemTra)
                                   .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<PhieuKiemTraDto> { Items = items, Total = total, Page = page, PageSize = pageSize };
        }

        public async Task<IEnumerable<PhieuKiemTraDto>> GetByThietBiAsync(int idThietBi)
            => await JoinQuery().Where(x => x.ID_ThietBi == idThietBi)
                                .OrderByDescending(x => x.NgayKiemTra).ToListAsync();

        public async Task<IEnumerable<PhieuKiemTraDto>> GetByNgayAsync(DateTime tuNgay, DateTime denNgay)
            => await JoinQuery().Where(x => x.NgayKiemTra >= tuNgay && x.NgayKiemTra <= denNgay)
                                .OrderByDescending(x => x.NgayKiemTra).ToListAsync();

        public async Task<PhieuKiemTraDto?> GetByIdAsync(int id)
            => await JoinQuery().FirstOrDefaultAsync(x => x.ID_Phieu == id);

        public async Task<PhieuKiemTraDetailDto?> GetDetailAsync(int idPhieu)
        {
            var phieu = await JoinQuery().FirstOrDefaultAsync(x => x.ID_Phieu == idPhieu);
            if (phieu == null) return null;

            var chiTiets = await (from ct in _db.ChiTietKiemTras
                                  join c in _db.ChiTieus on ct.ID_ChiTieu equals c.ID_ChiTieu
                                  where ct.IDPhieu == idPhieu
                                  select new ChiTietKiemTraDto
                                  {
                                      ID_ChiTiet = ct.ID_ChiTiet,
                                      IDPhieu = ct.IDPhieu,
                                      ID_ChiTieu = ct.ID_ChiTieu,
                                      TenChiTieu = c.TenChiTieu ?? string.Empty,
                                      GiaTriNhap_So = ct.GiaTriNhap_So,
                                      GiaTriNhap_Chu = ct.GiaTriNhap_Chu,
                                      Diem_Si_DatDuoc = ct.Diem_Si_DatDuoc,
                                      GhiChu = ct.GhiChu
                                  }).ToListAsync();

            return new PhieuKiemTraDetailDto
            {
                ID_Phieu = phieu.ID_Phieu,
                ID_ThietBi = phieu.ID_ThietBi,
                TenThietBi = phieu.TenThietBi,
                NgayKiemTra = phieu.NgayKiemTra,
                NguoiKiemTra = phieu.NguoiKiemTra,
                TongDiem_Soqt = phieu.TongDiem_Soqt,
                CapDoCanhBao = phieu.CapDoCanhBao,
                GhiChuChung = phieu.GhiChuChung,
                ChiTiets = chiTiets
            };
        }

        public async Task<PhieuKiemTraDto> CreateAsync(CreatePhieuKiemTraDto dto)
        {
            var phieu = new PhieuKiemTra
            {
                ID_ThietBi     = dto.ID_ThietBi,
                ID_NhomChiTieu = dto.ID_NhomChiTieu,
                NgayKiemTra    = DateTime.Now,
                NguoiKiemTra   = dto.NguoiKiemTra,
                GhiChuChung    = dto.GhiChuChung,
                SoPhieu        = await GenerateSoPhieuAsync(dto.ID_ThietBi, DateTime.Now)
            };
            await _phieuRepo.AddAsync(phieu);
            await _phieuRepo.SaveChangesAsync();

            // Tải LoaiTinhDiem cho các chỉ tiêu trong phiếu
            var allCtIds = dto.ChiTiets.Select(x => x.ID_ChiTieu).ToList();
            var chiTieuMetas = await _db.ChiTieus
                .Where(c => allCtIds.Contains(c.ID_ChiTieu))
                .Select(c => new { c.ID_ChiTieu, c.LoaiTinhDiem })
                .ToListAsync();

            // Index input definitions theo ID_ChiTieu để tránh N+1 cho Rule criteria
            var ruleCtIds = chiTieuMetas
                .Where(m => m.LoaiTinhDiem == "Rule")
                .Select(m => m.ID_ChiTieu)
                .ToList();

            Dictionary<int, List<CBM_ChiTieu_Input>> inputDefsMap = new();
            Dictionary<int, List<CBM_ChiTieu_Rule>> rulesMap = new();
            if (ruleCtIds.Count > 0 && dto.ChiTietInputs.Count > 0)
            {
                var allInputDefs = await _db.ChiTieuInputs
                    .Where(i => ruleCtIds.Contains(i.ID_ChiTieu))
                    .ToListAsync();
                inputDefsMap = allInputDefs.GroupBy(i => i.ID_ChiTieu)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var allRules = await _db.ChiTieuRules
                    .Where(r => ruleCtIds.Contains(r.ID_ChiTieu))
                    .ToListAsync();
                rulesMap = allRules.GroupBy(r => r.ID_ChiTieu)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            // Lưu chi tiết và thu thập (idChiTieu, diemDat) để tính CSSK
            var ketQuaChiTiets = new List<(int IdChiTieu, decimal? DiemDat)>();
            foreach (var ct in dto.ChiTiets)
            {
                var loaiTinhDiem = chiTieuMetas.FirstOrDefault(m => m.ID_ChiTieu == ct.ID_ChiTieu)?.LoaiTinhDiem;
                decimal? diemDat = null;

                if (loaiTinhDiem == "Rule"
                    && inputDefsMap.TryGetValue(ct.ID_ChiTieu, out var inputDefs)
                    && inputDefs.Count > 0)
                {
                    // Xây biến dict từ giá trị đã nộp (Rule type — có ID_Input từ DB)
                    var vars = new Dictionary<string, decimal>();
                    foreach (var def in inputDefs)
                    {
                        var submitted = dto.ChiTietInputs.FirstOrDefault(v => v.ID_Input == def.ID_Input);
                        if (submitted != null)
                        {
                            vars[def.MaInput] = submitted.GiaTriSo;
                            await _chiTietInputRepo.AddAsync(new ChiTietKiemTra_Input
                            {
                                IDPhieu    = phieu.ID_Phieu,
                                ID_ChiTieu = ct.ID_ChiTieu,
                                ID_Input   = def.ID_Input,
                                MaInput    = def.MaInput,
                                GiaTriSo   = submitted.GiaTriSo
                            });
                        }
                    }

                    // Đánh giá từng rule theo thứ tự Diem_Si giảm dần
                    if (vars.Count > 0 && rulesMap.TryGetValue(ct.ID_ChiTieu, out var rules))
                    {
                        foreach (var rule in rules.OrderByDescending(r => r.Diem_Si))
                        {
                            if (NguongEvaluator.EvalNCalc(rule.BieuThuc, vars))
                            {
                                diemDat = rule.Diem_Si;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    // Nguong với nhiều biến: frontend gửi theo tên (ChiTietInputsNamed)
                    // khi threshold dùng BieuThuc_Logic chứa nhiều ẩn số
                    var namedInputs = dto.ChiTietInputsNamed
                        .Where(x => x.ID_ChiTieu == ct.ID_ChiTieu)
                        .ToList();

                    if (namedInputs.Count > 0)
                    {
                        var vars = namedInputs.ToDictionary(x => x.MaInput, x => x.GiaTriSo);
                        var nguongs = await _nguongRepo.GetByChiTieuAsync(ct.ID_ChiTieu);
                        foreach (var ng in nguongs.OrderByDescending(n => n.Diem_Si))
                        {
                            bool matches = !string.IsNullOrWhiteSpace(ng.BieuThuc_Logic)
                                ? NguongEvaluator.EvalNCalc(ng.BieuThuc_Logic, vars)
                                : NguongEvaluator.KiemTraRange(
                                    vars.Count == 1 ? vars.Values.First() : null, ng);
                            if (matches) { diemDat = ng.Diem_Si; break; }
                        }

                        // Lưu từng biến đầu vào synthetic vào ChiTietKiemTra_Input
                        foreach (var ni in namedInputs)
                        {
                            await _chiTietInputRepo.AddAsync(new ChiTietKiemTra_Input
                            {
                                IDPhieu    = phieu.ID_Phieu,
                                ID_ChiTieu = ct.ID_ChiTieu,
                                MaInput    = ni.MaInput,
                                GiaTriSo   = ni.GiaTriSo
                            });
                        }
                    }
                    else
                    {
                        diemDat = await TinhDiemAsync(ct.ID_ChiTieu, ct.GiaTriNhap_So);
                    }
                }

                var chiTiet = new ChiTietKiemTra
                {
                    IDPhieu         = phieu.ID_Phieu,
                    ID_ChiTieu      = ct.ID_ChiTieu,
                    GiaTriNhap_So   = ct.GiaTriNhap_So,
                    GiaTriNhap_Chu  = ct.GiaTriNhap_Chu,
                    Diem_Si_DatDuoc = diemDat,
                    GhiChu          = ct.GhiChu
                };
                await _chiTietRepo.AddAsync(chiTiet);
                ketQuaChiTiets.Add((ct.ID_ChiTieu, diemDat));
            }

            // Tính CSSK theo công thức có trọng số (kết quả 0-100)
            //phieu.TongDiem_Soqt = await TinhCSSKAsync(ketQuaChiTiets);
            //phieu.CapDoCanhBao  = XepLoaiCapDo(phieu.TongDiem_Soqt ?? 0);
            _phieuRepo.Update(phieu);
            await _phieuRepo.SaveChangesAsync();

            return (await GetByIdAsync(phieu.ID_Phieu))!;
        }

        public async Task<PhieuKiemTraDto?> UpdateAsync(int id, UpdatePhieuKiemTraDto dto)
        {
            var entity = await _phieuRepo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ID_ThietBi     = dto.ID_ThietBi;
            entity.ID_NhomChiTieu = dto.ID_NhomChiTieu;
            entity.NgayKiemTra    = dto.NgayKiemTra;
            entity.NguoiKiemTra   = dto.NguoiKiemTra;
            entity.TongDiem_Soqt  = dto.TongDiem_Soqt;
            entity.CapDoCanhBao   = dto.CapDoCanhBao;
            entity.GhiChuChung    = dto.GhiChuChung;
            _phieuRepo.Update(entity);
            await _phieuRepo.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _phieuRepo.GetByIdAsync(id);
            if (entity == null) return false;

            await _chiTietRepo.DeleteByPhieuAsync(id);
            await _chiTietInputRepo.DeleteByPhieuAsync(id);
            _phieuRepo.Delete(entity);
            await _phieuRepo.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Sinh mã phiếu theo format: PKT-{KyHieu}-{yyyyMMdd}-{STT:D3}
        /// Ví dụ: PKT-MBA01-20260410-003
        /// </summary>
        private async Task<string> GenerateSoPhieuAsync(int idThietBi, DateTime ngay)
        {
            // Lấy ký hiệu thiết bị (KyHieu của LoaiThietBi hoặc SoHieu của ThietBi)
            var tb = await _db.ThietBis
                .Where(t => t.ID_ThietBi == idThietBi)
                .Select(t => new { t.TenThietBi, t.ID_LoaiTB })
                .FirstOrDefaultAsync();

            string kyHieu = "TB";
            if (tb != null)
            {
                if (!string.IsNullOrWhiteSpace(tb.TenThietBi))
                {
                    kyHieu = tb.TenThietBi.Trim();
                }
                else
                {
                    var loai = await _db.LoaiThietBis
                        .Where(l => l.ID_LoaiThietBi == tb.ID_LoaiTB)
                        .Select(l => l.KyHieu)
                        .FirstOrDefaultAsync();
                    if (!string.IsNullOrWhiteSpace(loai))
                        kyHieu = loai!.Trim();
                }
            }

            // Chuẩn hoá: bỏ khoảng trắng, ký tự đặc biệt
            kyHieu = System.Text.RegularExpressions.Regex.Replace(kyHieu, @"[^\w]", "").ToUpper();

            string ngayStr = ngay.ToString("yyyyMMdd");

            // STT = số phiếu của thiết bị này trong ngày hôm nay + 1
            var ngayBatDau = ngay.Date;
            var ngayKetThuc = ngayBatDau.AddDays(1);
            var soPhieuHomNay = await _db.PhieuKiemTras
                .CountAsync(p => p.ID_ThietBi == idThietBi
                              && p.NgayKiemTra >= ngayBatDau
                              && p.NgayKiemTra < ngayKetThuc);

            int stt = soPhieuHomNay + 1;

            return $"PKT-{kyHieu}-{ngayStr}-{stt:D3}";
        }

        private async Task<decimal?> TinhDiemAsync(int idChiTieu, decimal? giaTriSo)
        {
            if (giaTriSo == null) return null;
            var nguongs = await _nguongRepo.GetByChiTieuAsync(idChiTieu);
            return nguongs
                .OrderByDescending(ng => ng.Diem_Si)
                .FirstOrDefault(ng => NguongEvaluator.KiemTraNguongVoiGiaTri(giaTriSo, ng))
                ?.Diem_Si;
        }

    }
}
