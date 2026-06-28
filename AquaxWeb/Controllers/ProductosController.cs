using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Administrador")]
public class ProductosController : Controller
{
    private readonly AquaxDbContext _context;

    public ProductosController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.Productos.OrderBy(p => p.Nombre).ToListAsync());
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Crear(string nombre, string formato, decimal precio, int stock)
    {
        _context.Productos.Add(new Producto
        {
            Nombre = nombre,
            Formato = formato,
            Precio = precio,
            Stock = stock
        });
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Producto creado correctamente.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Editar(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        return View(producto ?? new Producto());
    }

    [HttpPost]
    public async Task<IActionResult> Editar(int id, string nombre, string formato, decimal precio, int stock)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto == null) return NotFound();

        producto.Nombre = nombre;
        producto.Formato = formato;
        producto.Precio = precio;
        producto.Stock = stock;

        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Producto actualizado correctamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        var producto = await _context.Productos.FindAsync(id);
        if (producto != null)
        {
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "Producto eliminado correctamente.";
        }
        return RedirectToAction("Index");
    }
}
