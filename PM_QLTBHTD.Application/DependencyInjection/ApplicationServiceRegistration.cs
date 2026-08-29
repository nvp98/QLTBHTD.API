using Microsoft.Extensions.DependencyInjection;
using PM_QLTBHTD.Application.Services;
using PM_QLTBHTD.Application.Services.IService;

namespace PM_QLTBHTD.Application.DependencyInjection
{
    /// <summary>Đăng ký toàn bộ Service (Application layer) — gọi 1 dòng duy nhất từ Program.cs
    /// thay vì rải ~25 dòng AddScoped ở đó, cho dễ đọc/dễ soát khi thêm/xóa service.</summary>
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Xác thực & phân quyền
            services.AddScoped<IVaiTroService, VaiTroService>();
            services.AddScoped<INguoiDungService, NguoiDungService>();
            services.AddScoped<IAuthService, AuthService>();

            // Danh mục
            services.AddScoped<IKhuVucService, KhuVucService>();
            services.AddScoped<ILoaiThietBiService, LoaiThietBiService>();
            services.AddScoped<ITramDienService, TramDienService>();
            services.AddScoped<INganLoService, NganLoService>();
            services.AddScoped<IThietBiService, ThietBiService>();

            // Chỉ tiêu
            services.AddScoped<INhomChiTieuService, NhomChiTieuService>();
            services.AddScoped<IChiTieuService, ChiTieuService>();
            services.AddScoped<INguongService, NguongService>();
            services.AddScoped<INguongScoringService, NguongScoringService>();
            services.AddScoped<IChiTieuInputService, ChiTieuInputService>();
            services.AddScoped<IThietBiThongSoService, ThietBiThongSoService>();
            services.AddScoped<IThongSoService, ThongSoService>();
            services.AddScoped<IChiTieuRuleService, ChiTieuRuleService>();
            services.AddScoped<IChiTieuFormulaService, ChiTieuFormulaService>();
            services.AddScoped<IChiTieuFormulaThamSoService, ChiTieuFormulaThamSoService>();
            services.AddScoped<IFormulaFunctionRegistry, FormulaFunctionRegistry>();
            services.AddScoped<IFormulaEngine, FormulaEngine>();
            services.AddScoped<IChiTieuPhanLoaiNguongService, ChiTieuPhanLoaiNguongService>();

            // Công thức tổng hợp
            services.AddScoped<ICongThucTongHopService, CongThucTongHopService>();
            services.AddScoped<ICongThucBienService, CongThucBienService>();
            services.AddScoped<ICongThucTestCaseService, CongThucTestCaseService>();
            services.AddScoped<ILichSuService, LichSuService>();

            // Phiếu kiểm tra & scoring
            services.AddScoped<IPhieuKiemTraService, PhieuKiemTraService>();
            services.AddScoped<IChiTietKiemTraService, ChiTietKiemTraService>();
            services.AddScoped<IChiTieuScoringService, ChiTieuScoringService>();
            services.AddScoped<IScoringEngine, ScoringEngine>();

            // Báo cáo
            services.AddScoped<IThongKeService, ThongKeService>();

            // Bảo trì
            services.AddScoped<ILichBaoTriService, LichBaoTriService>();

            return services;
        }
    }
}
