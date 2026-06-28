using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class Cliente
{
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Telefono { get; set; } = string.Empty;

    [Required]
    public string Direccion { get; set; } = string.Empty;

    public string? Email { get; set; }

    [Required]
    public string Correo { get; set; } = string.Empty;

    [Required]
    public string Contrasena { get; set; } = string.Empty;

    [Required]
    public string TipoCliente { get; set; } = "Natural";

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public int SaldoEnvases { get; set; } = 0;

    public decimal SaldoMonetario { get; set; } = 0;

    public int PuntosAqua { get; set; } = 0;
    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    public ICollection<DireccionCliente> Direcciones { get; set; } = new List<DireccionCliente>();
}
