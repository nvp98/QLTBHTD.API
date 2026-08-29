using PM_QLTBHTD.Domain.Entities;

namespace PM_QLTBHTD.Application.Services.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(CBM_NguoiDung nguoiDung, string maVaiTro);
    }
}
