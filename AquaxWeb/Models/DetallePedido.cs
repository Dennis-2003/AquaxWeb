using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class DetallePedido
{
    public int Id { get; set; }

    [Required]
    public int PedidoId { get; set; }

    [Required]
    public int ProductoId { get; set; }

    [Required]
    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public Pedido Pedido { get; set; } = null!;
    public Producto Producto { get; set; } = null!;
}
