using System.ComponentModel.DataAnnotations;

namespace AquaxWeb.Models;

public class HistorialAccesibilidad
{
    public int Id { get; set; }

    [Required]
    public int UsuarioId { get; set; }

    public bool NarradorActivado { get; set; }

    public bool ModoOscuro { get; set; }

    public int NivelZoom { get; set; } = 100;

    public string? FiltroColor { get; set; }

    public Usuario Usuario { get; set; } = null!;
}
