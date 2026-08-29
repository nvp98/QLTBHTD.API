using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PM_QLTBHTD.Application.DependencyInjection;
using PM_QLTBHTD.Application.Interfaces;
using PM_QLTBHTD.Application.Options;
using PM_QLTBHTD.Infrastructure.DependencyInjection;
using PM_QLTBHTD.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnectionString")));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Xác thực & phân quyền (JWT)
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
        };
    });
builder.Services.AddAuthorization();

// Repositories + Services — đăng ký gọn qua 2 extension method riêng (xem
// PM_QLTBHTD.Infrastructure/DependencyInjection và PM_QLTBHTD.Application/DependencyInjection)
// thay vì rải ~50 dòng AddScoped trực tiếp ở đây.
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();

// Yêu cầu đăng nhập mặc định cho MỌI controller/action — chỉ [AllowAnonymous] (vd AuthController.Login)
// mới bỏ qua; các action ghi dữ liệu dùng thêm [Authorize(Roles = "...")] để thu hẹp theo module.
builder.Services.AddControllers(o => o.Filters.Add(new AuthorizeFilter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CBM - Kiểm tra sức khỏe thiết bị điện EVN", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập JWT token (không cần tiền tố 'Bearer ')",
    });
    c.AddSecurityRequirement(new()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
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
// KHÔNG dùng UseHttpsRedirection(): FE gọi API qua HTTP thuần (VITE_API_URL=http://localhost:5180).
// Redirect 307 sang HTTPS (port khác) khiến trình duyệt tự xóa header Authorization khi theo
// redirect cross-origin — mọi request kèm Bearer token sau đó bị 401 dù token hợp lệ.
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
