using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/chitieu-formula-thamso")]
    [Authorize(Roles = "Admin,KySuCauHinh,GiamDoc")]
    public class ChiTieuFormulaThamSoController : ControllerBase
    {
        private readonly IChiTieuFormulaThamSoService _service;

        public ChiTieuFormulaThamSoController(IChiTieuFormulaThamSoService service)
            => _service = service;

        [HttpGet("by-formula/{idFormula}")]
        public async Task<IActionResult> GetByFormula(int idFormula)
            => Ok(await _service.GetByFormulaAsync(idFormula));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("create-chitieuformulathamso")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Create([FromBody] CreateChiTieuFormulaThamSoDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_ThamSo }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChiTieuFormulaThamSoDto dto)
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
    }
}
