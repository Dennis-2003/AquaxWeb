using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class Pedido
{
    public int Id { get; set; }

    [Required, MaxLength(15)]
    public string CodigoRastreo { get; set; } = null!;

    [Required]
    public int ClienteId { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime FechaPedido { get; set; } = DateTime.Now;
    
    public DateTime? FechaEntrega { get; set; }

    [Required]
    public string Estado { get; set; } = "Pendiente";

    public decimal MontoTotal { get; set; }

    public int? MetodoPagoId { get; set; }

    public int EnvasesEntregados { get; set; } = 0;

    public int EnvasesRecibidos { get; set; } = 0;

    [MaxLength(50)]
    public string? Zona { get; set; }

    public Cliente Cliente { get; set; } = null!;
    public Usuario? Usuario { get; set; }
    public MetodoPago? MetodoPago { get; set; }
    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
}
