using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartMovieApp.Data;
using SmartMovieApp.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// HttpClient - OMDb API için
builder.Services.AddHttpClient("OMDbClient", client =>
{
    client.BaseAddress = new Uri("http://www.omdbapi.com/");
});

// Authentication - Cookie öncelikli
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "SmartMovieApp",
        ValidAudience = "SmartMovieUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("bu-cok-gizli-ve-en-az-32-karakter-olan-bir-keydir!"))
    };
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Session
builder.Services.AddSession();

// MVC
builder.Services.AddControllersWithViews();

// Servisler
builder.Services.AddScoped<MovieService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();            // SESSION ÖNCE
app.UseAuthentication();     // AUTHENTICATION
app.UseAuthorization();      // AUTHORIZATION

// Varsayýlan route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
