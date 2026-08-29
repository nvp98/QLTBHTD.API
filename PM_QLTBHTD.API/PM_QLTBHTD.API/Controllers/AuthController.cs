using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly INguoiDungService _nguoiDungService;

        public AuthController(IAuthService authService, INguoiDungService nguoiDungService)
        {
            _authService = authService;
            _nguoiDungService = nguoiDungService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (SaiTaiKhoanMatKhauException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
            catch (TaiKhoanBiKhoaException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpGet("by-me")]
        public async Task<IActionResult> Me()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim, out var id)) return Unauthorized();

            var nguoiDung = await _nguoiDungService.GetByIdAsync(id);
            return nguoiDung == null ? Unauthorized() : Ok(nguoiDung);
        }

        [HttpPost("doi-mat-khau")]
        public async Task<IActionResult> DoiMatKhau([FromBody] DoiMatKhauDto dto)
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (idClaim == null || !int.TryParse(idClaim, out var id)) return Unauthorized();

            try
            {
                await _nguoiDungService.DoiMatKhauAsync(id, dto);
                return NoContent();
            }
            catch (MatKhauCuKhongDungException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
