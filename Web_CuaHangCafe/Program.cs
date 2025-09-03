using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_CuaHangCafe.Data;
using Web_CuaHangCafe.Repository;

var builder = WebApplication.CreateBuilder(args);

// 🔹 1. Kết nối SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 🔹 2. Cấu hình Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(120);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 🔹 3. Cấu hình Authentication Cookie (nếu dùng Auth mặc định)
builder.Services.AddAuthentication("AdminScheme")
    .AddCookie("AdminScheme", options =>
    {
        options.LoginPath = "/Accounts/Login";
        options.LogoutPath = "/Accounts/Logout";
        options.AccessDeniedPath = "/Accounts/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// 🔹 4. Đăng ký các dịch vụ phụ trợ
builder.Services.AddScoped<INhomSpRepository, NhomSpRepository>();
builder.Services.AddScoped<ShoppingCartSummaryViewComponent>();

// 🔹 5. Thêm MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 6. Kích hoạt Session và Authentication
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



// 🔹 7. Cấu hình route cho Area
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=HomeAdmin}/{action=Index}/{id?}");

// 🔹 8. Route mặc định
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
