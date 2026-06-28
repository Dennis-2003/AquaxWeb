using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;

namespace AquaxWeb.Controllers;

public class PedidoClienteController : Controller
{
    private readonly AquaxDbContext _context;

    public PedidoClienteController(AquaxDbContext context)
    {
        _context = context;
    }

    private static string GenerarCodigo()
    {
        const string chars = "0123456789ABCDEFGHJKLMNPQRSTUVWXYZ";
        var random = Random.Shared;
        var code = new char[8];
        for (int i = 0; i < 8; i++)
            code[i] = chars[random.Next(chars.Length)];
        return "AQUAX-" + new string(code);
    }

    [Authorize]
    public async Task<IActionResult> Crear()
    {
        ViewBag.Productos = await _context.Productos.ToListAsync();
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Crear(string nombre, string telefono, string direccion, string zona, Dictionary<int, int> cantidades)
    {
        var productosDb = await _context.Productos.ToListAsync();
        ViewBag.Productos = productosDb;

        // Filtrar solo productos con cantidad > 0
        var items = cantidades
            .Where(c => c.Value > 0)
            .Select(c => new { ProductoId = c.Key, Cantidad = c.Value })
            .ToList();

        if (items.Count == 0)
        {
            ModelState.AddModelError("", "Selecciona al menos un producto.");
            return View();
        }

        // Ahora el cliente SIEMPRE debe estar logueado
        int? clienteLogueadoId = User.FindFirst("ClienteId")?.Value is string cid ? int.Parse(cid) : null;

        if (clienteLogueadoId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var cliente = await _context.Clientes.FindAsync(clienteLogueadoId.Value);

        if (cliente == null)
        {
            return RedirectToAction("Login", "Account");
        }

        decimal total = 0;
        var detalles = new List<DetallePedido>();

        foreach (var item in items)
        {
            var producto = productosDb.FirstOrDefault(p => p.Id == item.ProductoId);
            if (producto == null) continue;

            total += producto.Precio * item.Cantidad;
            detalles.Add(new DetallePedido
            {
                ProductoId = item.ProductoId,
                Cantidad = item.Cantidad,
                PrecioUnitario = producto.Precio
            });
        }

        string codigoRastreo;
        do
        {
            codigoRastreo = GenerarCodigo();
        } while (await _context.Pedidos.AnyAsync(p => p.CodigoRastreo == codigoRastreo));

        var pedido = new Pedido
        {
            CodigoRastreo = codigoRastreo,
            ClienteId = cliente.Id,
            FechaPedido = DateTime.Now,
            Estado = "Pendiente",
            MontoTotal = total,
            Zona = zona,
            DetallesPedido = detalles
        };
        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = $"¡Pedido registrado con éxito!";
        TempData["CodigoRastreo"] = codigoRastreo;
        return RedirectToAction("Crear");
    }

    [HttpGet]
    public IActionResult MisPedidos()
    {
        var clienteId = User.FindFirst("ClienteId")?.Value;
        if (clienteId == null)
            return RedirectToAction("Login", "Account");

        var pedidos = _context.Pedidos
            .Where(p => p.ClienteId == int.Parse(clienteId))
            .Include(p => p.Usuario)
            .Include(p => p.DetallesPedido)
            .ThenInclude(dp => dp.Producto)
            .OrderByDescending(p => p.FechaPedido)
            .ToList();

        return View(pedidos);
    }

    [HttpGet]
    public IActionResult Rastrear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Rastrear(string? telefono, string? codigoRastreo)
    {
        List<Pedido> pedidos = new();
        Cliente? cliente = null;

        if (!string.IsNullOrEmpty(codigoRastreo))
        {
            var codigo = codigoRastreo.Trim().ToUpper();
            var pedido = await _context.Pedidos
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .Include(p => p.DetallesPedido)
                .ThenInclude(dp => dp.Producto)
                .FirstOrDefaultAsync(p => p.CodigoRastreo == codigo);

            if (pedido == null)
            {
                ViewBag.Error = $"No se encontró un pedido con el código \"{codigoRastreo}\".";
                return View();
            }

            cliente = pedido.Cliente;
            pedidos = new() { pedido };
        }
        else if (!string.IsNullOrEmpty(telefono))
        {
            cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Telefono == telefono);
            if (cliente == null)
            {
                ViewBag.Error = "No se encontraron pedidos con ese teléfono.";
                return View();
            }

            pedidos = await _context.Pedidos
                .Where(p => p.ClienteId == cliente.Id)
                .Include(p => p.Usuario)
                .Include(p => p.DetallesPedido)
                .ThenInclude(dp => dp.Producto)
                .OrderByDescending(p => p.FechaPedido)
                .ToListAsync();
        }
        else
        {
            ViewBag.Error = "Ingresa un código de rastreo o un teléfono.";
            return View();
        }

        ViewBag.Cliente = cliente;
        return View(pedidos);
    }
}