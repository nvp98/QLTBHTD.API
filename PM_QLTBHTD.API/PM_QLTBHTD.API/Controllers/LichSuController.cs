using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/lich-su")]
    public class LichSuController : ControllerBase
    {
        private readonly ILichSuService _service;

        public LichSuController(ILichSuService service) => _service = service;

        [HttpGet("{idThietBi}/{idNhomChiTieu}")]
        public async Task<IActionResult> GetLichSu(int idThietBi, int idNhomChiTieu)
        {
            var result = await _service.GetLichSuAsync(idThietBi, idNhomChiTieu);
            return result == null ? NotFound() : Ok(result);
        }
    }
}
