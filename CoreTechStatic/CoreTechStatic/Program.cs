using CoreTechStatic.Models.DateBase;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(option =>
{
    option.Cookie.IsEssential = true;
    option.SlidingExpiration = true;
    option.Cookie.Name = "AdminCookie";
    option.Cookie.MaxAge = TimeSpan.FromDays(360);
    option.ExpireTimeSpan = TimeSpan.FromDays(360);
    option.LoginPath = "/User/Login";
    option.AccessDeniedPath = "/User/UserAccessDenied";
});

builder.Services.AddAntiforgery(
    options =>
    {
        options.Cookie.Expiration = TimeSpan.Zero;
    });
builder.Services.AddDbContext<DataConn>(options => options.UseSqlServer
(builder.Configuration.GetConnectionString("Mycon")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
