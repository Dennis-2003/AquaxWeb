using AquaxWeb.Models;

namespace AquaxWeb.Services;

public interface IAccessibilidadService
{
    Task<HistorialAccesibilidad?> ObtenerConfiguracionAsync(int usuarioId);
}
