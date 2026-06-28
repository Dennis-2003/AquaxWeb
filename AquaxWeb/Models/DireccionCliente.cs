using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class DireccionCliente
{
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Required, MaxLength(255)]
    public string DireccionText { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Referencia { get; set; }

    public bool EsPrincipal { get; set; } = false;

    public Cliente Cliente { get; set; } = null!;
}
