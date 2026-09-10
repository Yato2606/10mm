using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Conectar la aplicación con SQL Server
builder.Services.AddDbContext<_10mm.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- CONFIGURACIÓN DE COOKIES DE SESIÓN ---
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuarios/Login"; // Redirige aquí si alguien intenta entrar a un área protegida
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Duración de la sesión activa
        options.AccessDeniedPath = "/Usuarios/Login";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// 1. Primero se lee quién es el usuario (Authentication)
app.UseAuthentication();

// 2. Luego se revisa a qué tiene permiso (Authorization)
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();