using Microsoft.AspNetCore.Mvc;
using _10mm.Data;
using _10mm.Models;
using Microsoft.EntityFrameworkCore;

namespace _10mm.Controllers
{
    public class TalleresController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inyectamos el contexto de la base de datos que configuramos antes
        public TalleresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Muestra la pantalla de registro (GET)
        public IActionResult Registrar()
        {
            return View();
        }

        // 2. Recibe los datos que el usuario escribe en el formulario y los guarda (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar([Bind("Nombre,Direccion,Telefono")] Taller taller)
        {
            if (ModelState.IsValid)
            {
                taller.FechaRegistro = DateTime.Now;
                _context.Add(taller);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            // 🛑 Si llegó aquí, significa que la validación falló. 
            // Esto va a forzar a que los errores aparezcan arriba en tu formulario en rojo:
            ModelState.AddModelError(string.Empty, "El modelo no es válido. Revisa los campos obligatorios.");

            return View(taller);
        }
    }
}