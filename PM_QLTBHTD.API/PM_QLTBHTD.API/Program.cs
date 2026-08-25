using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services;
using PM_QLTBHTD.Application.Services.IService;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;
using PM_QLTBHTD.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString")));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Repositories — danh mục
builder.Services.AddScoped<IKhuVucRepository, KhuVucRepository>();
builder.Services.AddScoped<ILoaiThietBiRepository, LoaiThietBiRepository>();
builder.Services.AddScoped<ITramDienRepository, TramDienRepository>();
builder.Services.AddScoped<INganLoRepository, NganLoRepository>();
builder.Services.AddScoped<IThietBiRepository, ThietBiRepository>();

// Repositories — chỉ tiêu đánh giá
builder.Services.AddScoped<INhomChiTieuRepository, NhomChiTieuRepository>();
builder.Services.AddScoped<IChiTieuRepository, ChiTieuRepository>();
builder.Services.AddScoped<INguongRepository, NguongRepository>();
builder.Services.AddScoped<IChiTieuInputRepository, ChiTieuInputRepository>();
builder.Services.AddScoped<IThietBiThongSoRepository, ThietBiThongSoRepository>();
builder.Services.AddScoped<IThongSoRepository, ThongSoRepository>();
builder.Services.AddScoped<IChiTieuRuleRepository, ChiTieuRuleRepository>();
builder.Services.AddScoped<IChiTieuFormulaRepository, ChiTieuFormulaRepository>();
builder.Services.AddScoped<IChiTieuFormulaThamSoRepository, ChiTieuFormulaThamSoRepository>();

// Repositories — công thức tổng hợp
builder.Services.AddScoped<ICongThucTongHopRepository, CongThucTongHopRepository>();
builder.Services.AddScoped<ICongThucBienRepository, CongThucBienRepository>();
builder.Services.AddScoped<IKetQuaNhomRepository, KetQuaNhomRepository>();
builder.Services.AddScoped<IKetQuaTrungGianRepository, KetQuaTrungGianRepository>();
builder.Services.AddScoped<ICongThucTestCaseRepository, CongThucTestCaseRepository>();

// Repositories — phiếu kiểm tra
builder.Services.AddScoped<IPhieuKiemTraRepository, PhieuKiemTraRepository>();
builder.Services.AddScoped<IChiTietKiemTraRepository, ChiTietKiemTraRepository>();
builder.Services.AddScoped<IChiTietKiemTraInputRepository, ChiTietKiemTraInputRepository>();

// Repositories — phân loại theo thời gian
builder.Services.AddScoped<IPhanLoaiNguongRepository, PhanLoaiNguongRepository>();
builder.Services.AddScoped<IKetQuaPhanLoaiThangRepository, KetQuaPhanLoaiThangRepository>();

// Repositories — bảo trì
builder.Services.AddScoped<ILichBaoTriRepository, LichBaoTriRepository>();

// Services — danh mục
builder.Services.AddScoped<IKhuVucService, KhuVucService>();
builder.Services.AddScoped<ILoaiThietBiService, LoaiThietBiService>();
builder.Services.AddScoped<ITramDienService, TramDienService>();
builder.Services.AddScoped<INganLoService, NganLoService>();
builder.Services.AddScoped<IThietBiService, ThietBiService>();

// Services — chỉ tiêu
builder.Services.AddScoped<INhomChiTieuService, NhomChiTieuService>();
builder.Services.AddScoped<IChiTieuService, ChiTieuService>();
builder.Services.AddScoped<INguongService, NguongService>();
builder.Services.AddScoped<INguongScoringService, NguongScoringService>();
builder.Services.AddScoped<IChiTieuInputService, ChiTieuInputService>();
builder.Services.AddScoped<IThietBiThongSoService, ThietBiThongSoService>();
builder.Services.AddScoped<IThongSoService, ThongSoService>();
builder.Services.AddScoped<IChiTieuRuleService, ChiTieuRuleService>();
builder.Services.AddScoped<IChiTieuFormulaService, ChiTieuFormulaService>();
builder.Services.AddScoped<IChiTieuFormulaThamSoService, ChiTieuFormulaThamSoService>();
builder.Services.AddScoped<IFormulaFunctionRegistry, FormulaFunctionRegistry>();
builder.Services.AddScoped<IFormulaEngine, FormulaEngine>();
builder.Services.AddScoped<IChiTieuPhanLoaiNguongService, ChiTieuPhanLoaiNguongService>();

// Services — công thức tổng hợp
builder.Services.AddScoped<ICongThucTongHopService, CongThucTongHopService>();
builder.Services.AddScoped<ICongThucBienService, CongThucBienService>();
builder.Services.AddScoped<ICongThucTestCaseService, CongThucTestCaseService>();
builder.Services.AddScoped<ILichSuService, LichSuService>();

// Services — phiếu kiểm tra & scoring
builder.Services.AddScoped<IPhieuKiemTraService, PhieuKiemTraService>();
builder.Services.AddScoped<IChiTietKiemTraService, ChiTietKiemTraService>();
builder.Services.AddScoped<IChiTieuScoringService, ChiTieuScoringService>();
builder.Services.AddScoped<IScoringEngine, ScoringEngine>();

// Services — báo cáo
builder.Services.AddScoped<IThongKeService, ThongKeService>();

// Services — bảo trì
builder.Services.AddScoped<ILichBaoTriService, LichBaoTriService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CBM - Kiểm tra sức khỏe thiết bị điện EVN", Version = "v1" });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CBM API v1"));


app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
