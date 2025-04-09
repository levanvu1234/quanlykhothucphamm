using Microsoft.EntityFrameworkCore;
using QuanLyKhoThucPham.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuanLyKhoThucPhamContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("QuanLyKhoThucPhamContext") ?? throw new InvalidOperationException("Connection string 'QuanLyKhoThucPhamContext' not found.")));

// Add services to the container.  
builder.Services.AddControllersWithViews();

// Add authentication services  
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Đường dẫn đến trang đăng nhập  
        options.AccessDeniedPath = "/Account/AccessDenied"; // Đường dẫn khi truy cập bị từ chối (tùy chọn)  
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.  
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Add authentication middleware **BEFORE** authorization  
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();