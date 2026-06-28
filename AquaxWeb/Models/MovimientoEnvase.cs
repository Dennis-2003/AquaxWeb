using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AquaxWeb.Models;

public class MovimientoEnvase
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }
    
    public int? PedidoId { get; set; }

    [Required]
    public DateTime Fecha { get; set; } = DateTime.Now;

    [Required]
    public int EnvasesEntregados { get; set; }

    [Required]
    public int EnvasesDevueltos { get; set; }

    [Required]
    public int SaldoResultante { get; set; }

    [MaxLength(200)]
    public string? Observacion { get; set; }

    [ForeignKey("ClienteId")]
    public virtual Cliente Cliente { get; set; } = null!;

    [ForeignKey("PedidoId")]
    public virtual Pedido? Pedido { get; set; }
}
