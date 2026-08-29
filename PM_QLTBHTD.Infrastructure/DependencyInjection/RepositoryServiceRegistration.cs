using Microsoft.Extensions.DependencyInjection;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Repository;
using PM_QLTBHTD.Infrastructure.Services;

namespace PM_QLTBHTD.Infrastructure.DependencyInjection
{
    /// <summary>Đăng ký toàn bộ Repository (+ các service triển khai ở Infrastructure như
    /// IJwtTokenGenerator) — gọi 1 dòng duy nhất từ Program.cs thay vì rải ~25 dòng AddScoped ở đó.</summary>
    public static class RepositoryServiceRegistration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            // Xác thực & phân quyền
            services.AddScoped<IVaiTroRepository, VaiTroRepository>();
            services.AddScoped<INguoiDungRepository, NguoiDungRepository>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Danh mục
            services.AddScoped<IKhuVucRepository, KhuVucRepository>();
            services.AddScoped<ILoaiThietBiRepository, LoaiThietBiRepository>();
            services.AddScoped<ITramDienRepository, TramDienRepository>();
            services.AddScoped<INganLoRepository, NganLoRepository>();
            services.AddScoped<IThietBiRepository, ThietBiRepository>();

            // Chỉ tiêu đánh giá
            services.AddScoped<INhomChiTieuRepository, NhomChiTieuRepository>();
            services.AddScoped<IChiTieuRepository, ChiTieuRepository>();
            services.AddScoped<INguongRepository, NguongRepository>();
            services.AddScoped<IChiTieuInputRepository, ChiTieuInputRepository>();
            services.AddScoped<IThietBiThongSoRepository, ThietBiThongSoRepository>();
            services.AddScoped<IThongSoRepository, ThongSoRepository>();
            services.AddScoped<IChiTieuRuleRepository, ChiTieuRuleRepository>();
            services.AddScoped<IChiTieuFormulaRepository, ChiTieuFormulaRepository>();
            services.AddScoped<IChiTieuFormulaThamSoRepository, ChiTieuFormulaThamSoRepository>();

            // Công thức tổng hợp
            services.AddScoped<ICongThucTongHopRepository, CongThucTongHopRepository>();
            services.AddScoped<ICongThucBienRepository, CongThucBienRepository>();
            services.AddScoped<IKetQuaNhomRepository, KetQuaNhomRepository>();
            services.AddScoped<IKetQuaTrungGianRepository, KetQuaTrungGianRepository>();
            services.AddScoped<ICongThucTestCaseRepository, CongThucTestCaseRepository>();

            // Phiếu kiểm tra
            services.AddScoped<IPhieuKiemTraRepository, PhieuKiemTraRepository>();
            services.AddScoped<IChiTietKiemTraRepository, ChiTietKiemTraRepository>();
            services.AddScoped<IChiTietKiemTraInputRepository, ChiTietKiemTraInputRepository>();

            // Phân loại theo thời gian
            services.AddScoped<IPhanLoaiNguongRepository, PhanLoaiNguongRepository>();
            services.AddScoped<IKetQuaPhanLoaiThangRepository, KetQuaPhanLoaiThangRepository>();

            // Bảo trì
            services.AddScoped<ILichBaoTriRepository, LichBaoTriRepository>();

            return services;
        }
    }
}
