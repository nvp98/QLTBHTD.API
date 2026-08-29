using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/thietbi-thongso")]
    [Authorize(Roles = "Admin,KySuCauHinh,GiamDoc")]
    public class ThietBiThongSoController : ControllerBase
    {
        private readonly IThietBiThongSoService _service;

        public ThietBiThongSoController(IThietBiThongSoService service) => _service = service;

        [HttpGet("by-thietbi/{idThietBi}")]
        public async Task<IActionResult> GetThietBiThongSoByThietBi(int idThietBi)
            => Ok(await _service.GetByThietBiAsync(idThietBi));

        [HttpGet("by-thongso/{idThongSo}")]
        public async Task<IActionResult> GetThietBiThongSoByThongSo(int idThongSo)
            => Ok(await _service.GetByThongSoAsync(idThongSo));

        [HttpPost("create-thietbithongso")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> CreateThietBiThongSo([FromBody] CreateThietBiThongSoDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> UpdateThietBiThongSo(int id, [FromBody] UpdateThietBiThongSoDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> DeleteThietBiThongSo(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}
