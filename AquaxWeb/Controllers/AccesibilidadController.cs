using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;
using AquaxWeb.Models.ViewModels;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Administrador,Trabajador")]
public class AccesibilidadController : Controller
{
    private readonly AquaxDbContext _context;

    public AccesibilidadController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);
        var config = await _context.HistorialAccesibilidad
            .FirstOrDefaultAsync(h => h.UsuarioId == usuarioId);

        return View(config ?? new HistorialAccesibilidad
        {
            NivelZoom = 100,
            NarradorActivado = false,
            ModoOscuro = false
        });
    }

    [HttpPost]
    public async Task<IActionResult> Guardar([FromBody] AccesibilidadRequest request)
    {
        var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);
        var config = await _context.HistorialAccesibilidad
            .FirstOrDefaultAsync(h => h.UsuarioId == usuarioId);

        if (config == null)
        {
            config = new HistorialAccesibilidad { UsuarioId = usuarioId };
            _context.HistorialAccesibilidad.Add(config);
        }

        config.NarradorActivado = request.NarradorActivado;
        config.ModoOscuro = request.ModoOscuro;
        config.NivelZoom = request.NivelZoom;
        config.FiltroColor = request.FiltroColor;

        await _context.SaveChangesAsync();
        return Json(new { ok = true });
    }
}
