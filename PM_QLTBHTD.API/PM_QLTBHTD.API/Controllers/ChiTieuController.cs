using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,KySuCauHinh,GiamDoc")]
    public class ChiTieuController : ControllerBase
    {
        private readonly IChiTieuService _service;

        public ChiTieuController(IChiTieuService service)
        {
            _service = service;
        }

        [HttpGet("get-all-chitieu")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? idNhom,
            [FromQuery] int? idLoai,
            [FromQuery] int page = 1,
            [FromQuery] int? pageSize = null)
            => Ok(await _service.GetPagedAsync(search, idNhom, idLoai, page, pageSize));

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
            => Ok(await _service.GetAllActiveAsync());

        [HttpGet("by-nhom/{idNhomChiTieu}")]
        public async Task<IActionResult> GetByNhom(int idNhomChiTieu)
            => Ok(await _service.GetByNhomChiTieuAsync(idNhomChiTieu));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("create-chitieu")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Create([FromBody] CreateChiTieuDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_ChiTieu }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateChiTieuDto dto)
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
            catch (ChiTieuDangSuDungException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
