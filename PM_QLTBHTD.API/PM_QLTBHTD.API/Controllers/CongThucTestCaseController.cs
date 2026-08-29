using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/cong-thuc-testcase")]
    [Authorize(Roles = "Admin,KySuCauHinh,GiamDoc")]
    public class CongThucTestCaseController : ControllerBase
    {
        private readonly ICongThucTestCaseService _service;

        public CongThucTestCaseController(ICongThucTestCaseService service)
        {
            _service = service;
        }

        [HttpGet("by-congthuc/{idCongThuc}")]
        public async Task<IActionResult> GetByCongThuc(int idCongThuc)
            => Ok(await _service.GetByCongThucAsync(idCongThuc));

        [HttpPost("create-congthuctestcase")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Create([FromBody] CreateCongThucTestCaseDto dto)
            => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCongThucTestCaseDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? NoContent() : NotFound();
        }

        [HttpPost("run/{idCongThuc}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Run(int idCongThuc)
            => Ok(await _service.ChayTatCaAsync(idCongThuc));
    }
}
