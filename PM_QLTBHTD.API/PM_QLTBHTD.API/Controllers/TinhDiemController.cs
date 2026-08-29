using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/phieu-kiem-tra")]
    public class TinhDiemController : ControllerBase
    {
        private readonly IScoringEngine _engine;
        private readonly IChiTieuScoringService _scoringService;

        public TinhDiemController(IScoringEngine engine, IChiTieuScoringService scoringService)
        {
            _engine = engine;
            _scoringService = scoringService;
        }

        /// <summary>
        /// Tính điểm một nhóm chỉ tiêu cụ thể cho phiếu kiểm tra.
        /// Kết quả được cache trong CBM_KetQuaNhom.
        /// </summary>
        [HttpGet("tinh-diem-nhom/{idNhomChiTieu}/{idPhieu}")]
        public async Task<IActionResult> TinhDiemNhom(int idPhieu, int idNhomChiTieu, CancellationToken ct)
        {
            try
            {
                var diem = await _engine.TinhDiemNhomAsync(idNhomChiTieu, idPhieu, ct);
                return Ok(new { IDPhieu = idPhieu, ID_NhomChiTieu = idNhomChiTieu, Diem = diem });
            }
            catch (CongThucKhongTonTaiException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdNhomChiTieu });
            }
            catch (ThieuDuLieuChiTietKiemTraException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdChiTieu, ex.IdPhieu });
            }
            catch (VongLapNhomChiTieuException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdNhomBiLap, DuongDi = ex.DuongDi });
            }
        }

        /// <summary>
        /// Tính lại CSSK đại diện cho cả phiếu và ghi vào PhieuKiemTra.TongDiem_Soqt.
        /// Dùng để "làm mới" sau khi sửa công thức/cây chỉ tiêu hoặc bổ sung dữ liệu còn thiếu.
        /// </summary>
        [HttpPost("tinh-tong-diem/{idPhieu}")]
        [Authorize(Roles = "Admin,KyThuatVien,TruongTram")]
        public async Task<IActionResult> TinhTongDiem(int idPhieu, CancellationToken ct)
        {
            try
            {
                var diem = await _engine.TinhVaLuuTongDiemAsync(idPhieu, ct);
                return Ok(new { IDPhieu = idPhieu, TongDiem_Soqt = diem });
            }
            catch (NhieuNhomGocException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdLoaiThietBi, ex.SoLuongGoc });
            }
        }

        /// <summary>
        /// Tính lại Sᵢ + khuyến cáo hành động cho TẤT CẢ chỉ tiêu đã có dữ liệu trong phiếu (dùng dữ liệu
        /// đã lưu, không cần nhập lại), rồi tính lại CSSK tổng luôn. Khác "tinh-tong-diem": endpoint đó
        /// chỉ gộp lại từ Sᵢ CŨ đã lưu — không tính lại Sᵢ, nên không phản ánh cấu hình Rule/Formula/Ngưỡng
        /// vừa sửa. Dùng endpoint này sau khi sửa cấu hình tính điểm để "làm mới" 1 phiếu đã tồn tại.
        /// </summary>
        [HttpPost("tinh-lai-si/{idPhieu}")]
        [Authorize(Roles = "Admin,KyThuatVien,TruongTram")]
        public async Task<IActionResult> TinhLaiSi(int idPhieu, CancellationToken ct)
        {
            try
            {
                await _scoringService.TinhLaiToanBoSiAsync(idPhieu, ct);
                var diem = await _engine.TinhVaLuuTongDiemAsync(idPhieu, ct);
                return Ok(new { IDPhieu = idPhieu, TongDiem_Soqt = diem });
            }
            catch (NhieuNhomGocException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdLoaiThietBi, ex.SoLuongGoc });
            }
        }

        /// <summary>
        /// Tính toàn bộ cây chỉ số sức khỏe cho phiếu kiểm tra.
        /// Trả về cây kết quả lồng nhau.
        /// </summary>
        [HttpGet("tinh-chi-so-suc-khoe/{idPhieu}")]
        public async Task<IActionResult> TinhChiSoSucKhoe(int idPhieu, [FromQuery] int idLoaiThietBi, CancellationToken ct)
        {
            try
            {
                var cayKetQua = await _engine.TinhDiemCayAsync(idLoaiThietBi, idPhieu, ct);
                return Ok(new TinhDiemCayResultDto
                {
                    IDPhieu = idPhieu,
                    KetQuaCay = cayKetQua.ToList()
                });
            }
            catch (ThieuDuLieuChiTietKiemTraException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdChiTieu, ex.IdPhieu });
            }
            catch (VongLapNhomChiTieuException ex)
            {
                return UnprocessableEntity(new { Error = ex.Message, ex.IdNhomBiLap, DuongDi = ex.DuongDi });
            }
        }
    }
}
