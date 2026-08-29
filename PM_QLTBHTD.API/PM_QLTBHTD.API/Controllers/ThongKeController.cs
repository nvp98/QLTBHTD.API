using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _service;

        public ThongKeController(IThongKeService service)
        {
            _service = service;
        }

        /// <summary>Tổng hợp CSSK toàn hệ thống</summary>
        [HttpGet("tong-hop")]
        public async Task<IActionResult> GetTongHop()
            => Ok(await _service.GetTongHopAsync());

        /// <summary>Lịch sử CSSK của một thiết bị theo thời gian</summary>
        [HttpGet("lich-su-thiet-bi/{idThietBi:int}")]
        public async Task<IActionResult> GetLichSuThietBi(int idThietBi)
            => Ok(await _service.GetLichSuThietBiAsync(idThietBi));

        /// <summary>Báo cáo sức khỏe tổng hợp theo trạm điện</summary>
        [HttpGet("bao-cao-tram")]
        public async Task<IActionResult> GetBaoCaoTram()
            => Ok(await _service.GetBaoCaoTramAsync());

        /// <summary>CSSK trung bình + phân bố hạng theo từng loại thiết bị (thay cho 1 số gộp toàn hệ thống)</summary>
        [HttpGet("tong-hop-theo-loai")]
        public async Task<IActionResult> GetTongHopTheoLoai()
            => Ok(await _service.GetTongHopTheoLoaiAsync());

        /// <summary>Danh sách thiết bị cần chú ý/cảnh báo (CSSK &lt; 70)</summary>
        [HttpGet("canh-bao")]
        public async Task<IActionResult> GetCanhBao()
            => Ok(await _service.GetCanhBaoAsync());

        /// <summary>Xu hướng CSSK trung bình theo tháng (mặc định 6 tháng gần nhất) — có thể lọc theo
        /// trạm điện, loại thiết bị, và/hoặc 1 thiết bị cụ thể (thuộc trạm/loại đã chọn).</summary>
        [HttpGet("xu-huong-thang")]
        public async Task<IActionResult> GetXuHuongThang(
            [FromQuery] int soThang = 6, [FromQuery] int? idTram = null,
            [FromQuery] int? idLoaiTB = null, [FromQuery] int? idThietBi = null)
            => Ok(await _service.GetXuHuongThangAsync(soThang, idTram, idLoaiTB, idThietBi));
    }
}
