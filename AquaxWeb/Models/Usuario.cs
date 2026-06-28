using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Contrasena { get; set; } = string.Empty;

    [Required]
    public string Rol { get; set; } = "Trabajador";

    public string? Telefono { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public HistorialAccesibilidad? HistorialAccesibilidad { get; set; }
    public ICollection<HistorialSesion> HistorialSesiones { get; set; } = new List<HistorialSesion>();
}
