using Microsoft.EntityFrameworkCore;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Services;
using PM_QLTBHTD.Domain.IRepository;
using PM_QLTBHTD.Infrastructure.Persistence;
using PM_QLTBHTD.Infrastructure.Repository;
using PM_QLTBHTD.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString")));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Repositories — danh mục
builder.Services.AddScoped<IKhuVucRepository, KhuVucRepository>();
builder.Services.AddScoped<ILoaiThietBiRepository, LoaiThietBiRepository>();
builder.Services.AddScoped<ITramDienRepository, TramDienRepository>();
builder.Services.AddScoped<IThietBiRepository, ThietBiRepository>();

// Repositories — chỉ tiêu đánh giá
builder.Services.AddScoped<INhomChiTieuRepository, NhomChiTieuRepository>();
builder.Services.AddScoped<IChiTieuRepository, ChiTieuRepository>();
builder.Services.AddScoped<INguongRepository, NguongRepository>();
builder.Services.AddScoped<IChiTieuInputRepository, ChiTieuInputRepository>();
builder.Services.AddScoped<IChiTieuRuleRepository, ChiTieuRuleRepository>();

// Repositories — công thức tổng hợp
builder.Services.AddScoped<ICongThucTongHopRepository, CongThucTongHopRepository>();
builder.Services.AddScoped<ICongThucBienRepository, CongThucBienRepository>();
builder.Services.AddScoped<IKetQuaNhomRepository, KetQuaNhomRepository>();

// Repositories — phiếu kiểm tra
builder.Services.AddScoped<IPhieuKiemTraRepository, PhieuKiemTraRepository>();
builder.Services.AddScoped<IChiTietKiemTraRepository, ChiTietKiemTraRepository>();
builder.Services.AddScoped<IChiTietKiemTraInputRepository, ChiTietKiemTraInputRepository>();

// Repositories — phân loại theo thời gian
builder.Services.AddScoped<IPhanLoaiNguongRepository, PhanLoaiNguongRepository>();
builder.Services.AddScoped<IKetQuaPhanLoaiThangRepository, KetQuaPhanLoaiThangRepository>();

// Services — danh mục
builder.Services.AddScoped<IKhuVucService, KhuVucService>();
builder.Services.AddScoped<ILoaiThietBiService, LoaiThietBiService>();
builder.Services.AddScoped<ITramDienService, TramDienService>();
builder.Services.AddScoped<IThietBiService, ThietBiService>();

// Services — chỉ tiêu
builder.Services.AddScoped<INhomChiTieuService, NhomChiTieuService>();
builder.Services.AddScoped<IChiTieuService, ChiTieuService>();
builder.Services.AddScoped<INguongService, NguongService>();
builder.Services.AddScoped<INguongScoringService, NguongScoringService>();
builder.Services.AddScoped<IChiTieuInputService, ChiTieuInputService>();
builder.Services.AddScoped<IChiTieuRuleService, ChiTieuRuleService>();

// Services — công thức tổng hợp
builder.Services.AddScoped<ICongThucTongHopService, CongThucTongHopService>();
builder.Services.AddScoped<ICongThucBienService, CongThucBienService>();

// Services — phiếu kiểm tra & scoring
builder.Services.AddScoped<IPhieuKiemTraService, PhieuKiemTraService>();
builder.Services.AddScoped<IChiTietKiemTraService, ChiTietKiemTraService>();
builder.Services.AddScoped<IChiTieuScoringService, ChiTieuScoringService>();
builder.Services.AddScoped<IScoringEngine, ScoringEngine>();

// Services — báo cáo
builder.Services.AddScoped<IThongKeService, ThongKeService>();

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
