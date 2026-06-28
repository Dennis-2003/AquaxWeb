using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Administrador,Trabajador")]
public class PerfilController : Controller
{
    private readonly AquaxDbContext _context;

    public PerfilController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        return View(usuario ?? new Usuario());
    }

    [HttpPost]
    public async Task<IActionResult> Guardar(string nombre, string? telefono)
    {
        var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);
        var usuario = await _context.Usuarios.FindAsync(usuarioId);

        if (usuario != null)
        {
            usuario.Nombre = nombre;
            usuario.Telefono = telefono;
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Perfil actualizado correctamente.";
        }

        return RedirectToAction("Index");
    }
}
