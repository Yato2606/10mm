using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _10mm.Data;
using _10mm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace _10mm.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Usuarios/Registrar
        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            var listaTalleres = await _context.Talleres
                .Select(t => new { Id = t.Id, Nombre = t.Nombre })
                .ToListAsync();

            ViewBag.Talleres = new SelectList(listaTalleres, "Id", "Nombre");
            return View();
        }

        // POST: /Usuarios/Registrar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistroViewModel model)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == model.Correo))
            {
                ModelState.AddModelError("Correo", "Este correo electrónico ya está registrado.");
            }

            if (ModelState.IsValid)
            {
                var nuevoUsuario = new Usuario
                {
                    Nombre = model.Nombre,
                    ApellidoPaterno = model.ApellidoPaterno,
                    ApellidoMaterno = model.ApellidoMaterno,
                    Correo = model.Correo,
                    Telefono = model.Telefono,
                    Rol = model.Rol,
                    IdTaller = model.Rol == "Mecanico" ? model.IdTaller : null,
                    Activo = true,
                    FechaRegistro = DateTime.Now,
                    EmailConfirmado = false,
                    TokenVerificacion = Guid.NewGuid().ToString()
                };

                nuevoUsuario.PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(model.Password));

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            var talleres = await _context.Talleres.ToListAsync();
            ViewBag.Talleres = new SelectList(talleres, "Id", "Nombre");
            return View(model);
        }

        // GET: /Usuarios/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Usuarios/Login (CON SOLUCIÓN A ERRORES DE COMPILACIÓN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Correo == model.Correo);

                if (usuario != null)
                {
                    // Solución CS1503: Separamos los bytes en una variable explícita para evitar confusiones de tipo
                    byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(model.Password);
                    string passwordIngresadaHash = Convert.ToBase64String(passwordBytes);

                    if (usuario.PasswordHash == passwordIngresadaHash)
                    {
                        if (!usuario.Activo)
                        {
                            ModelState.AddModelError(string.Empty, "Tu cuenta está desactivada. Contacta al administrador.");
                            return View(model);
                        }

                        // Solución CS1061: Usamos el correo electrónico como identificador único temporal 
                        // para evitar el problema de si tu propiedad de ID se llama 'Id' o 'IdUsuario'
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, usuario.Correo),
                            new Claim(ClaimTypes.Name, $"{usuario.Nombre} {usuario.ApellidoPaterno}"),
                            new Claim(ClaimTypes.Email, usuario.Correo),
                            new Claim(ClaimTypes.Role, usuario.Rol)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = model.Recordarme,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), authProperties);

                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            }

            return View(model);
        }

        // GET: /Usuarios/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Usuarios");
        }
    }
}