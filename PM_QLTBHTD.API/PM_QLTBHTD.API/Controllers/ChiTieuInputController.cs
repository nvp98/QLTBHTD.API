using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/chitieu-input")]
    [Authorize(Roles = "Admin,KySuCauHinh,GiamDoc")]
    public class ChiTieuInputController : ControllerBase
    {
        private readonly IChiTieuInputService _service;

        public ChiTieuInputController(IChiTieuInputService service)
            => _service = service;

        [HttpGet("by-chitieu/{idChiTieu}")]
        public async Task<IActionResult> GetByChiTieu(int idChiTieu)
            => Ok(await _service.GetByChiTieuAsync(idChiTieu));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("create-chitieuinput")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Create([FromBody] CreateChiTieuInputDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_Input }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChiTieuInputDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _service.DeleteAsync(id);
                return result ? NoContent() : NotFound();
            }
            catch (ChiTieuInputDangSuDungException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
