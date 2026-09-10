using Microsoft.EntityFrameworkCore;
using _10mm.Models;

namespace _10mm.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Taller> Talleres { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}