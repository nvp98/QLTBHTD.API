using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class PhieuKiemTraService : IPhieuKiemTraService
    {
        private readonly IPhieuKiemTraRepository _phieuRepo;
        private readonly IChiTietKiemTraRepository _chiTietRepo;
        private readonly IChiTietKiemTraInputRepository _chiTietInputRepo;
        private readonly IKetQuaNhomRepository _ketQuaNhomRepo;
        private readonly IKetQuaPhanLoaiThangRepository _ketQuaPhanLoaiThangRepo;
        private readonly ILichBaoTriRepository _lichBaoTriRepo;
        private readonly IAppDbContext _db;
        private readonly IChiTieuScoringService _scoringService;
        private readonly IScoringEngine _scoringEngine;

        public PhieuKiemTraService(
            IPhieuKiemTraRepository phieuRepo,
            IChiTietKiemTraRepository chiTietRepo,
            IChiTietKiemTraInputRepository chiTietInputRepo,
            IKetQuaNhomRepository ketQuaNhomRepo,
            IKetQuaPhanLoaiThangRepository ketQuaPhanLoaiThangRepo,
            ILichBaoTriRepository lichBaoTriRepo,
            IAppDbContext db,
            IChiTieuScoringService scoringService,
            IScoringEngine scoringEngine)
        {
            _phieuRepo = phieuRepo;
            _chiTietRepo = chiTietRepo;
            _chiTietInputRepo = chiTietInputRepo;
            _ketQuaNhomRepo = ketQuaNhomRepo;
            _ketQuaPhanLoaiThangRepo = ketQuaPhanLoaiThangRepo;
            _lichBaoTriRepo = lichBaoTriRepo;
            _db = db;
            _scoringService = scoringService;
            _scoringEngine = scoringEngine;
        }

        private IQueryable<PhieuKiemTraDto> JoinQuery()
        {
            return from p in _db.PhieuKiemTras
                   join tb in _db.ThietBis on p.ID_ThietBi equals tb.ID_ThietBi
                   join tram in _db.TramDiens on tb.ID_Tram equals tram.IDTram
                   join loai in _db.LoaiThietBis on tb.ID_LoaiTB equals loai.ID_LoaiThietBi
                   join nhom in _db.NhomChiTieus on p.ID_NhomChiTieu equals nhom.ID_NhomChiTieu into nhomJoin
                   from nhom in nhomJoin.DefaultIfEmpty()
                   select new PhieuKiemTraDto
                   {
                       ID_Phieu       = p.ID_Phieu,
                       ID_ThietBi     = p.ID_ThietBi,
                       TenThietBi     = tb.TenThietBi,
                       ID_Tram        = tb.ID_Tram,
                       TenTram        = tram.TenTram,
                       ID_LoaiTB      = tb.ID_LoaiTB,
                       TenLoaiTB      = loai.TenLoaiTB,
                       ID_NhomChiTieu = p.ID_NhomChiTieu,
                       TenNhom        = nhom != null ? nhom.TenNhom : null,
                       NgayKiemTra    = p.NgayKiemTra,
                       NguoiKiemTra   = p.NguoiKiemTra,
                       TongDiem_Soqt  = p.TongDiem_Soqt,
                       CapDoCanhBao   = p.CapDoCanhBao,
                       GhiChuChung    = p.GhiChuChung
                   };
        }

        /// <summary>Áp bộ lọc dùng chung cho danh sách kết quả kiểm tra (trang Kết quả) — Trạm/Loại
        /// thiết bị/Thiết bị/khoảng ngày kiểm tra, cộng tìm kiếm tự do theo tên thiết bị/KTV. Ngày
        /// so sánh theo NGÀY LỊCH (bỏ phần giờ) để FE khỏi phải tự tính start/end-of-day.</summary>
        private static IQueryable<PhieuKiemTraDto> ApplyFilter(
            IQueryable<PhieuKiemTraDto> query,
            string? search, int? idTram, int? idLoaiTB, int? idThietBi, DateTime? tuNgay, DateTime? denNgay)
        {
            if (!string.IsNullOrEmpty(search))
                query = query.Where(x => x.TenThietBi.Contains(search) || (x.NguoiKiemTra != null && x.NguoiKiemTra.Contains(search)));
            if (idTram.HasValue)
                query = query.Where(x => x.ID_Tram == idTram.Value);
            if (idLoaiTB.HasValue)
                query = query.Where(x => x.ID_LoaiTB == idLoaiTB.Value);
            if (idThietBi.HasValue)
                query = query.Where(x => x.ID_ThietBi == idThietBi.Value);
            if (tuNgay.HasValue)
                query = query.Where(x => x.NgayKiemTra >= tuNgay.Value.Date);
            if (denNgay.HasValue)
                query = query.Where(x => x.NgayKiemTra < denNgay.Value.Date.AddDays(1));
            return query;
        }

        public async Task<PagedResult<PhieuKiemTraDto>> GetPagedAsync(
            string? search, int? idTram, int? idLoaiTB, int? idThietBi, DateTime? tuNgay, DateTime? denNgay,
            int page, int? pageSize)
        {
            var query = ApplyFilter(JoinQuery(), search, idTram, idLoaiTB, idThietBi, tuNgay, denNgay);

            var total = await query.CountAsync();
            var ordered = query.OrderByDescending(x => x.NgayKiemTra).ThenByDescending(x => x.ID_Phieu);
            var items = await (pageSize.HasValue
                ? ordered.Skip((page - 1) * pageSize.Value).Take(pageSize.Value)
                : ordered).ToListAsync();
            return new PagedResult<PhieuKiemTraDto> { Items = items, Total = total, Page = page, PageSize = pageSize ?? total };
        }

        /// <summary>Phiếu mới nhất của mỗi thiết bị trong tập ĐÃ LỌC — lọc trước, chọn mới nhất mỗi
        /// nhóm sau (khớp đúng hành vi trước đây khi làm phía client). Dùng subquery MAX(ID_Phieu)
        /// GROUP BY ID_ThietBi thay vì GroupBy+First trên DTO phức tạp — dịch sang SQL ổn định hơn.</summary>
        public async Task<IEnumerable<PhieuKiemTraDto>> GetLatestPerThietBiAsync(
            string? search, int? idTram, int? idLoaiTB, int? idThietBi, DateTime? tuNgay, DateTime? denNgay)
        {
            var filtered = ApplyFilter(JoinQuery(), search, idTram, idLoaiTB, idThietBi, tuNgay, denNgay);

            var latestIdPhieus = filtered
                .GroupBy(x => x.ID_ThietBi)
                .Select(g => g.Max(x => x.ID_Phieu));

            return await filtered
                .Where(x => latestIdPhieus.Contains(x.ID_Phieu))
                .OrderByDescending(x => x.NgayKiemTra)
                .ToListAsync();
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
                                  join n in _db.NhomChiTieus on c.ID_NhomChiTieu equals n.ID_NhomChiTieu
                                  where ct.IDPhieu == idPhieu
                                  select new ChiTietKiemTraDto
                                  {
                                      ID_ChiTiet = ct.ID_ChiTiet,
                                      IDPhieu = ct.IDPhieu,
                                      ID_ChiTieu = ct.ID_ChiTieu,
                                      TenChiTieu = c.TenChiTieu ?? string.Empty,
                                      ID_NhomChiTieu = n.ID_NhomChiTieu,
                                      TenNhom = n.TenNhom,
                                      GiaTriNhap_So = ct.GiaTriNhap_So,
                                      GiaTriNhap_Chu = ct.GiaTriNhap_Chu,
                                      Diem_Si_DatDuoc = ct.Diem_Si_DatDuoc,
                                      HanhDongKhuyenCao = ct.HanhDongKhuyenCao,
                                      GhiChu = ct.GhiChu
                                  }).ToListAsync();

            await GanDanhSachInputAsync(idPhieu, chiTiets, ct => ct.ID_ChiTieu, (ct, ds) => ct.DanhSachInput = ds);

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

        public async Task<List<LichSuChiTieuDto>> GetLichSuChiTieuAsync(int idThietBi, int idChiTieu)
        {
            var lichSu = await (from ct in _db.ChiTietKiemTras
                                join p in _db.PhieuKiemTras on ct.IDPhieu equals p.ID_Phieu
                                where p.ID_ThietBi == idThietBi && ct.ID_ChiTieu == idChiTieu
                                orderby p.NgayKiemTra descending
                                select new LichSuChiTieuDto
                                {
                                    ID_Phieu = p.ID_Phieu,
                                    NgayKiemTra = p.NgayKiemTra,
                                    GiaTriNhap_So = ct.GiaTriNhap_So,
                                    GiaTriNhap_Chu = ct.GiaTriNhap_Chu,
                                    Diem_Si_DatDuoc = ct.Diem_Si_DatDuoc,
                                    HanhDongKhuyenCao = ct.HanhDongKhuyenCao
                                }).ToListAsync();

            var idPhieus = lichSu.Select(x => x.ID_Phieu).ToList();
            var inputRows = await _db.ChiTietKiemTra_Inputs
                .Where(x => idPhieus.Contains(x.IDPhieu) && x.ID_ChiTieu == idChiTieu)
                .ToListAsync();
            if (inputRows.Count == 0) return lichSu;

            var inputDefs = await _db.ChiTieuInputs.Where(x => x.ID_ChiTieu == idChiTieu).ToListAsync();
            var tenTheoMa = inputDefs.ToDictionary(x => x.MaInput, x => x.TenInput);
            var defTheoId = inputDefs.ToDictionary(x => x.ID_Input, x => x);

            foreach (var muc in lichSu)
            {
                var rows = inputRows.Where(x => x.IDPhieu == muc.ID_Phieu).ToList();
                if (rows.Count == 0) continue;
                muc.DanhSachInput = rows.Select(r =>
                {
                    var ma = r.MaInput
                        ?? (r.ID_Input.HasValue && defTheoId.TryGetValue(r.ID_Input.Value, out var def) ? def.MaInput : "?");
                    tenTheoMa.TryGetValue(ma, out var ten);
                    return new ChiTietInputValueDto { MaInput = ma, TenInput = ten, GiaTriSo = r.GiaTriSo };
                }).ToList();
            }

            return lichSu;
        }

        /// <summary>Nạp CBM_ChiTietKiemTra_Input (giá trị Input thô, vd T_tren=48) vào DanhSachInput của
        /// các chỉ tiêu tương ứng trong 1 phiếu.</summary>
        private async Task GanDanhSachInputAsync<T>(
            int idPhieu, List<T> chiTiets, Func<T, int> idChiTieuCua, Action<T, List<ChiTietInputValueDto>> ganVao)
        {
            var idsChiTieu = chiTiets.Select(idChiTieuCua).Distinct().ToList();
            if (idsChiTieu.Count == 0) return;

            var inputRows = await _db.ChiTietKiemTra_Inputs
                .Where(x => x.IDPhieu == idPhieu && idsChiTieu.Contains(x.ID_ChiTieu))
                .ToListAsync();
            if (inputRows.Count == 0) return;

            var inputDefs = await _db.ChiTieuInputs.Where(x => idsChiTieu.Contains(x.ID_ChiTieu)).ToListAsync();
            var tenTheoMa = inputDefs.ToDictionary(x => (x.ID_ChiTieu, x.MaInput), x => x.TenInput);
            var defTheoId = inputDefs.ToDictionary(x => x.ID_Input, x => x);

            foreach (var ct in chiTiets)
            {
                var idChiTieu = idChiTieuCua(ct);
                var rows = inputRows.Where(x => x.ID_ChiTieu == idChiTieu).ToList();
                if (rows.Count == 0) continue;
                var danhSach = rows.Select(r =>
                {
                    var ma = r.MaInput
                        ?? (r.ID_Input.HasValue && defTheoId.TryGetValue(r.ID_Input.Value, out var def) ? def.MaInput : "?");
                    tenTheoMa.TryGetValue((idChiTieu, ma), out var ten);
                    return new ChiTietInputValueDto { MaInput = ma, TenInput = ten, GiaTriSo = r.GiaTriSo };
                }).ToList();
                ganVao(ct, danhSach);
            }
        }

        public async Task<PhieuKiemTraDto> CreateAsync(CreatePhieuKiemTraDto dto)
        {
            var ngayKiemTra = dto.NgayKiemTra ?? DateTime.Now;
            var phieu = new PhieuKiemTra
            {
                ID_ThietBi     = dto.ID_ThietBi,
                ID_NhomChiTieu = dto.ID_NhomChiTieu,
                NgayKiemTra    = ngayKiemTra,
                NguoiKiemTra   = dto.NguoiKiemTra,
                GhiChuChung    = dto.GhiChuChung,
                SoPhieu        = await GenerateSoPhieuAsync(dto.ID_ThietBi, ngayKiemTra)
            };
            await _phieuRepo.AddAsync(phieu);
            await _phieuRepo.SaveChangesAsync();

            // Map DTO tạo phiếu -> DTO nhập liệu dùng chung với ChiTieuScoringService, để chỉ còn
            // 1 nguồn tính Si duy nhất (Rule/LF/Formula/Nguong đều xử lý trong đó, kể cả khi tạo phiếu mới).
            var allCtIds = dto.ChiTiets.Select(x => x.ID_ChiTieu).ToList();
            var inputDefs = await _db.ChiTieuInputs
                .Where(i => allCtIds.Contains(i.ID_ChiTieu))
                .ToListAsync();
            var inputDefById = inputDefs.ToDictionary(i => i.ID_Input);

            var danhSachNhap = dto.ChiTiets.Select(ct =>
            {
                var vars = new Dictionary<string, decimal>();

                foreach (var v in dto.ChiTietInputs)
                    if (inputDefById.TryGetValue(v.ID_Input, out var def) && def.ID_ChiTieu == ct.ID_ChiTieu)
                        vars[def.MaInput] = v.GiaTriSo;

                foreach (var ni in dto.ChiTietInputsNamed)
                    if (ni.ID_ChiTieu == ct.ID_ChiTieu)
                        vars[ni.MaInput] = ni.GiaTriSo;

                return new NhapChiTietKiemTraDto
                {
                    ID_ChiTieu     = ct.ID_ChiTieu,
                    GiaTriNhap_So  = ct.GiaTriNhap_So,
                    GiaTriNhap_Chu = ct.GiaTriNhap_Chu,
                    GhiChu         = ct.GhiChu,
                    DanhSachInput  = vars.Count > 0 ? vars : null
                };
            }).ToList();

            await _scoringService.TinhVaLuuDiemSiAsync(phieu.ID_Phieu, danhSachNhap);

            // Tính và lưu CSSK tổng qua ScoringEngine (đệ quy cây chỉ tiêu từ nhóm gốc).
            // Không chặn tạo phiếu nếu cây chưa đủ công thức/dữ liệu — ScoringEngine tự trả null
            // cho các lỗi tính toán thông thường; chỉ NhieuNhomGocException (lỗi cấu hình loại
            // thiết bị có >1 nhóm gốc) là đáng chú ý nhưng cũng không nên chặn việc lưu phiếu.
            try
            {
                await _scoringEngine.TinhVaLuuTongDiemAsync(phieu.ID_Phieu);
            }
            catch (NhieuNhomGocException)
            {
                // Cấu hình cây có vấn đề (nhiều nhóm gốc) — để CSSK tổng trống, không chặn tạo phiếu.
            }

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

            // Cache/snapshot theo IDPhieu — dọn cùng để không để lại rác trỏ tới phiếu đã xóa.
            // (CBM_KetQuaTrungGian là audit trail APPEND-only theo thiết kế, cố tình KHÔNG xóa theo.)
            foreach (var kq in await _ketQuaNhomRepo.FindAsync(x => x.IDPhieu == id))
                _ketQuaNhomRepo.Delete(kq);
            await _ketQuaNhomRepo.SaveChangesAsync();

            foreach (var pl in await _ketQuaPhanLoaiThangRepo.FindAsync(x => x.IDPhieu == id))
                _ketQuaPhanLoaiThangRepo.Delete(pl);
            await _ketQuaPhanLoaiThangRepo.SaveChangesAsync();

            // LichBaoTri.ID_PhieuKetQua trỏ tới phiếu này (nullable) — gỡ tham chiếu tránh treo.
            foreach (var lbt in await _lichBaoTriRepo.FindAsync(x => x.ID_PhieuKetQua == id))
            {
                lbt.ID_PhieuKetQua = null;
                _lichBaoTriRepo.Update(lbt);
            }
            await _lichBaoTriRepo.SaveChangesAsync();

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

    }
}
