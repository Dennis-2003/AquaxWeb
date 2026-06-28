using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class Gasto
{
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public string Descripcion { get; set; } = string.Empty;

    public decimal Monto { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    [MaxLength(255)]
    public string? CodigoVerificacionInterno { get; set; }
}
