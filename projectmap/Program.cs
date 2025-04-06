using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using projectmap.Clouds;
using projectmap.Common;
using projectmap.Models;
using projectmap.Service;
using projectmap.ViewModel;
using Quartz;
using System.Runtime.InteropServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
#endregion

#region CORS
var corsBuilder = new CorsPolicyBuilder();
corsBuilder.AllowAnyHeader();
corsBuilder.AllowAnyMethod();
corsBuilder.AllowAnyOrigin();
//corsBuilder.WithOrigins("http://34.80.69.96:8080"); // Đây là Url bên frontEnd
//corsBuilder.WithOrigins("http://localhost:8080"); // Đây là Url bên frontEnd
corsBuilder.WithOrigins("https://5dc9-34-80-69-96.ngrok-free.app", "http://34.80.69.96:8080"); // Đây là Url bên frontEnd
corsBuilder.AllowCredentials();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", corsBuilder.Build());
});

#endregion

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CoreJwtExample",
        Version = "v1",
        Description = "Hello Anh Em",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
        {
            Name = "Thanh Toán Online",
            Url = new Uri("https://localhost:44316/")
        }
    });



    // Phần xác thực người dùng(JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.
                        Example: 'Bearer asddvsvs123'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

    //var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var path = Path.Combine(AppContext.BaseDirectory, xmlFileName);
    //c.IncludeXmlComments(path);
});

var connection = builder.Configuration.GetConnectionString("MyDB");
builder.Services.AddDbContext<DBContext>(option =>
{
    option.UseMySql(connection, new MySqlServerVersion(new Version(8, 0, 31))); // "ThuongMaiDienTu" đây là tên của project, vì tách riêng model khỏi project sang 1 lớp khác nên phải để câu lệnh này "b => b.MigrationsAssembly("ThuongMaiDienTu")"
});

builder.Services.AddSignalR();
// Đọc cấu hình Cloudinary từ appsettings.json
var cloudinaryAccount = new CloudinaryDotNet.Account(
    builder.Configuration["Cloud:Cloudinary_Name"],
    builder.Configuration["Cloud:Api_Key"],
    builder.Configuration["Cloud:Serec_Key"]
);
var cloudinary = new Cloudinary(cloudinaryAccount);

// Đăng ký Cloudinary làm một dịch vụ Singleton
builder.Services.AddSingleton(cloudinary);

builder.Services.Configure<Cloud>(builder.Configuration.GetSection("Cloud"));

//builder.Services.AddQuartz(q =>
//{
//    q.UseMicrosoftDependencyInjectionJobFactory();
//    var jobKey = new JobKey("TuDongMoiTuan");
//    q.AddJob<TuDongMoiTuan>(Otp => Otp.WithIdentity(jobKey));
//    q.AddTrigger(otps => otps.ForJob(jobKey).WithIdentity("WeeklyTrigger")
//    .StartNow()
//    .WithCronSchedule("0 0 0/1 * * ?"));
//    //.WithCronSchedule("0 0/1 * * * ?")); // "0/1" là chạy mỗi phút, để "1" là chỉ chạy 1 phút lần đầu
//});


builder.Services.AddSwaggerGen();
// Đăng ký HostedService cho Quartz.NET
//builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddAuthentication(); // Sử dụng phân quyền
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserTokenService, UserTokenService>();
builder.Services.AddScoped<ITrafficEquipmentService, TrafficEquipmentService>();
builder.Services.AddScoped<IRepairDetailsService, RepairDetailsService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(); // Sử dụng phân quyền
builder.Services.AddMvc();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddResponseCompression(); // ✅ Thêm dòng này

builder.WebHost.UseUrls("http://0.0.0.0:5000");
//builder.Services.AddSignalR(options =>
//{
//    options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // Giới hạn lưu trữ dung lượng file qua SignalR là 10MB
//});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();
app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseCookiePolicy();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
