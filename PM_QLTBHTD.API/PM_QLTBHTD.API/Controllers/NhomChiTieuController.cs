using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/nhom-chi-tieu")]
    [Authorize(Roles = "Admin,KySuCauHinh,GiamDoc")]
    public class NhomChiTieuController : ControllerBase
    {
        private readonly INhomChiTieuService _service;

        public NhomChiTieuController(INhomChiTieuService service)
        {
            _service = service;
        }

        [HttpGet("get-all-nhomchitieu")]
        public async Task<IActionResult> GetAll([FromQuery] string? search, int? id_LoaiTB, int? tramDien, [FromQuery] int page = 1, [FromQuery] int? pageSize = null)
            => Ok(await _service.GetPagedAsync(search, page, pageSize));

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
            => Ok(await _service.GetAllActiveAsync());

        [HttpGet("by-loaithietbi/{idLoaiThietBi}")]
        public async Task<IActionResult> GetByLoaiThietBi(int idLoaiThietBi)
            => Ok(await _service.GetByLoaiThietBiAsync(idLoaiThietBi));

        [HttpGet("kha-dung-nhap-lieu/{idLoaiThietBi}")]
        public async Task<IActionResult> GetKhaDungNhapLieu(int idLoaiThietBi)
            => Ok(await _service.GetKhaDungNhapLieuAsync(idLoaiThietBi));

        [HttpGet("cay/{idLoaiThietBi}")]
        public async Task<IActionResult> GetCay(int idLoaiThietBi)
            => Ok(await _service.GetCayAsync(idLoaiThietBi));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost("create-nhomchitieu")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Create([FromBody] CreateNhomChiTieuDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.ID_NhomChiTieu }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,KySuCauHinh")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNhomChiTieuDto dto)
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
            catch (NhomChiTieuDangSuDungException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
