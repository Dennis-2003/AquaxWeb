using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly AquaxDbContext _context;

    public AdminController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.Usuarios.OrderBy(u => u.Rol).ThenBy(u => u.Nombre).ToListAsync();
        return View(usuarios);
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Crear(string nombre, string correo, string contrasena, string rol)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Correo == correo))
        {
            ModelState.AddModelError("", "El correo ya está registrado.");
            return View();
        }

        _context.Usuarios.Add(new Usuario
        {
            Nombre = nombre,
            Correo = correo,
            Contrasena = contrasena,
            Rol = rol,
            FechaRegistro = DateTime.Now
        });

        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Trabajador creado correctamente.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Editar(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        return View(usuario ?? new Usuario());
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, string nombre, string correo, string rol)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        usuario.Nombre = nombre;
        usuario.Correo = correo;
        usuario.Rol = rol;

        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Trabajador actualizado correctamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Trabajador eliminado correctamente.";
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> CambiarContrasena(int id, string nuevaContrasena)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();

        usuario.Contrasena = nuevaContrasena;
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Contraseña cambiada correctamente.";
        return RedirectToAction("Index");
    }
}
