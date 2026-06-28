using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;

namespace AquaxWeb.Services;

public class AccessibilidadService : IAccessibilidadService
{
    private readonly AquaxDbContext _context;

    public AccessibilidadService(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<HistorialAccesibilidad?> ObtenerConfiguracionAsync(int usuarioId)
    {
        return await _context.HistorialAccesibilidad
            .FirstOrDefaultAsync(h => h.UsuarioId == usuarioId);
    }
}
