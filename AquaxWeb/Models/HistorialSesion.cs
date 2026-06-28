using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class HistorialSesion
{
    public int Id { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    public DateTime FechaIngreso { get; set; } = DateTime.Now;

    [MaxLength(200)]
    public string? Dispositivo { get; set; }

    [MaxLength(255)]
    public string? TokenVerificador { get; set; }

    public Usuario Usuario { get; set; } = null!;
}
