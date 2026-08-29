using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PM_QLTBHTD.Application.Options;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Infrastructure.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _options;

        public JwtTokenGenerator(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(CBM_NguoiDung nguoiDung, string maVaiTro)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, nguoiDung.ID_NguoiDung.ToString()),
                new(ClaimTypes.Name, nguoiDung.TenDangNhap),
                new(ClaimTypes.Role, maVaiTro),
            };
            if (nguoiDung.ID_Tram.HasValue)
                claims.Add(new Claim("id_tram", nguoiDung.ID_Tram.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpireMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
