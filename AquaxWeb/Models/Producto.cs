using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class Producto
{
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    public string Formato { get; set; } = string.Empty;

    [Required]
    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public ICollection<DetallePedido> DetallesPedido { get; set; } = new List<DetallePedido>();
}
