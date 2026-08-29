using PM_QLTBHTD.Application.DTOs;
using PM_QLTBHTD.Application.Exceptions;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.IRepository;

namespace PM_QLTBHTD.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly INguoiDungRepository _repository;
        private readonly INguoiDungService _nguoiDungService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(INguoiDungRepository repository, INguoiDungService nguoiDungService, IJwtTokenGenerator jwtTokenGenerator)
        {
            _repository = repository;
            _nguoiDungService = nguoiDungService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var entity = await _repository.GetByTenDangNhapAsync(dto.TenDangNhap)
                ?? throw new SaiTaiKhoanMatKhauException();

            if (!BCrypt.Net.BCrypt.Verify(dto.MatKhau, entity.MatKhau_Hash))
                throw new SaiTaiKhoanMatKhauException();

            if (entity.TrangThai != 1)
                throw new TaiKhoanBiKhoaException();

            var nguoiDungDto = await _nguoiDungService.GetByIdAsync(entity.ID_NguoiDung)
                ?? throw new SaiTaiKhoanMatKhauException();

            var token = _jwtTokenGenerator.GenerateToken(entity, nguoiDungDto.MaVaiTro);
            return new LoginResponseDto { Token = token, NguoiDung = nguoiDungDto };
        }

        public async Task<NguoiDungDto> GetByIdAsync(int idNguoiDung)
            => await _nguoiDungService.GetByIdAsync(idNguoiDung)
                ?? throw new SaiTaiKhoanMatKhauException();
    }
}
