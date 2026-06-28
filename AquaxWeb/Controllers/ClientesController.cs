using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Administrador")]
public class ClientesController : Controller
{
    private readonly AquaxDbContext _context;

    public ClientesController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Clientes.OrderByDescending(c => c.FechaRegistro).ToListAsync());
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Crear(string nombre, string telefono, string direccion, string? email)
    {
        _context.Clientes.Add(new Cliente
        {
            Nombre = nombre,
            Telefono = telefono,
            Direccion = direccion,
            Email = email
        });
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Cliente creado correctamente.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Editar(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        return View(cliente ?? new Cliente());
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, string nombre, string telefono, string direccion, string? email)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return NotFound();

        cliente.Nombre = nombre;
        cliente.Telefono = telefono;
        cliente.Direccion = direccion;
        cliente.Email = email;

        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Cliente actualizado correctamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Cliente eliminado correctamente.";
        }
        return RedirectToAction("Index");
    }
}
