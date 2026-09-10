using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _10mm.Models
{
    [Table("Usuarios", Schema = "dbo")]
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        public int? IdTaller { get; set; } // Nullable porque administradores o clientes pueden no tener taller asignado

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido paterno es obligatorio.")]
        [StringLength(100)]
        public string ApellidoPaterno { get; set; }

        [StringLength(100)]
        public string ApellidoMaterno { get; set; } // Opcional

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un correo válido.")]
        [StringLength(150)]
        public string Correo { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; } // 'Administrador', 'Mecanico', 'Cliente'

        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool EmailConfirmado { get; set; } = false;

        [StringLength(255)]
        public string TokenVerificacion { get; set; }

        // Propiedad de navegación virtual hacia el Taller (Opcional, útil para Entity Framework)
        [ForeignKey("IdTaller")]
        public virtual Taller Taller { get; set; }
    }
}