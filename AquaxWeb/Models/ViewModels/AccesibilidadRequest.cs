namespace AquaxWeb.Models.ViewModels;

public class AccesibilidadRequest
{
    public bool NarradorActivado { get; set; }
    public bool ModoOscuro { get; set; }
    public int NivelZoom { get; set; } = 100;
    public string? FiltroColor { get; set; }
}
