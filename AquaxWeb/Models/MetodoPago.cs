using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class MetodoPago
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
