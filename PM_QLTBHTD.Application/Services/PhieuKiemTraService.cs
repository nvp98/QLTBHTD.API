using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
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
        private readonly IAppDbContext _db;
        private readonly IChiTieuScoringService _scoringService;
        private readonly IScoringEngine _scoringEngine;

        public PhieuKiemTraService(
            IPhieuKiemTraRepository phieuRepo,
            IChiTietKiemTraRepository chiTietRepo,
            IChiTietKiemTraInputRepository chiTietInputRepo,
            IAppDbContext db,
            IChiTieuScoringService scoringService,
            IScoringEngine scoringEngine)
        {
            _phieuRepo = phieuRepo;
            _chiTietRepo = chiTietRepo;
            _chiTietInputRepo = chiTietInputRepo;
            _db = db;
            _scoringService = scoringService;
            _scoringEngine = scoringEngine;
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

        public async Task<PagedResult<PhieuKiemTraDto>> GetPagedAsync(string? search, int page, int? pageSize)
        {
            var query = JoinQuery().Where(x =>
                string.IsNullOrEmpty(search)
                || x.TenThietBi.Contains(search)
                || (x.NguoiKiemTra != null && x.NguoiKiemTra.Contains(search)));

            var total = await query.CountAsync();
            var ordered = query.OrderByDescending(x => x.NgayKiemTra);
            var items = await (pageSize.HasValue
                ? ordered.Skip((page - 1) * pageSize.Value).Take(pageSize.Value)
                : ordered).ToListAsync();
            return new PagedResult<PhieuKiemTraDto> { Items = items, Total = total, Page = page, PageSize = pageSize ?? total };
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
