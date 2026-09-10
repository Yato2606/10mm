using System.ComponentModel.DataAnnotations;

namespace _10mm.Models
{
    public class Taller
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del taller es obligatorio.")]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(20)]
        public string Telefono { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relación: Un taller tiene muchos usuarios
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}