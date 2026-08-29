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

        /// <summary>Danh sách thiết bị cần chú ý/cảnh báo (CSSK &lt; 70)</summary>
        [HttpGet("canh-bao")]
        public async Task<IActionResult> GetCanhBao()
            => Ok(await _service.GetCanhBaoAsync());
    }
}
