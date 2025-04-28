using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using tao_project.Data;
using tao_project.Models;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình DbContext với SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Cấu hình Identity với Role support
// builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => 
//     {
//         options.SignIn.RequireConfirmedAccount = true;
//         options.Password.RequireDigit = true;
//         options.Password.RequiredLength = 8;
//         options.Password.RequireNonAlphanumeric = false;
//         options.Password.RequireUppercase = true;
//         options.Password.RequireLowercase = true;
//     })
//     .AddEntityFrameworkStores<ApplicationDbContext>()
//     .AddDefaultTokenProviders()
//     .AddDefaultUI(); // Thêm UI mặc định của Identity

//Thêm các dịch vụ cần thiếtx
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();