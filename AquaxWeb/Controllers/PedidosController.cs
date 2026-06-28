using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;
using AquaxWeb.Models.ViewModels;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Administrador,Trabajador")]
public class PedidosController : Controller
{
    private readonly AquaxDbContext _context;

    public PedidosController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(DateTime? fechaDesde, DateTime? fechaHasta, string? estado)
    {
        var esAdmin = User.IsInRole("Administrador");
        var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);

        var query = _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Usuario)
            .AsQueryable();

        if (!esAdmin)
            query = query.Where(p => p.UsuarioId == usuarioId);

        if (fechaDesde.HasValue)
            query = query.Where(p => p.FechaPedido >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(p => p.FechaPedido <= fechaHasta.Value.AddDays(1));

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(p => p.Estado == estado);

        var modelo = new PedidoViewModel
        {
            Pedidos = await query.OrderByDescending(p => p.FechaPedido).ToListAsync(),
            Trabajadores = esAdmin
                ? await _context.Usuarios.Where(u => u.Rol == "Trabajador").ToListAsync()
                : new(),
            FechaDesde = fechaDesde,
            FechaHasta = fechaHasta,
            EstadoFiltro = estado,
            EsAdmin = esAdmin
        };

        return View(modelo);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Asignar(int pedidoId, int usuarioId)
    {
        var pedido = await _context.Pedidos.FindAsync(pedidoId);
        if (pedido == null) return NotFound();

        pedido.UsuarioId = usuarioId;
        pedido.Estado = "Pendiente";
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Pedido asignado correctamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Desasignar(int pedidoId)
    {
        var pedido = await _context.Pedidos.FindAsync(pedidoId);
        if (pedido == null) return NotFound();

        pedido.UsuarioId = null;
        pedido.Estado = "Pendiente";
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Pedido desasignado correctamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> CambiarEstado(int pedidoId, string estado)
    {
        var esAdmin = User.IsInRole("Administrador");
        var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);

        var pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null) return NotFound();

        if (!esAdmin && pedido.UsuarioId != usuarioId)
            return Forbid();

        pedido.Estado = estado;
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = $"Pedido marcado como '{estado}'.";
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Exportar(DateTime? fechaDesde, DateTime? fechaHasta, string? estado)
    {
        var esAdmin = User.IsInRole("Administrador");
        var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);

        var query = _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Usuario)
            .AsQueryable();

        if (!esAdmin)
            query = query.Where(p => p.UsuarioId == usuarioId);

        if (fechaDesde.HasValue)
            query = query.Where(p => p.FechaPedido >= fechaDesde.Value);

        if (fechaHasta.HasValue)
            query = query.Where(p => p.FechaPedido <= fechaHasta.Value.AddDays(1));

        if (!string.IsNullOrEmpty(estado))
            query = query.Where(p => p.Estado == estado);

        var pedidos = await query.OrderByDescending(p => p.FechaPedido).ToListAsync();

        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Id,Cliente,Repartidor,Fecha,Total,Estado");

        foreach (var p in pedidos)
        {
            var repartidor = p.Usuario?.Nombre ?? "Sin asignar";
            csv.AppendLine($"{p.Id},{p.Cliente?.Nombre},{repartidor},{p.FechaPedido:yyyy-MM-dd HH:mm},{p.MontoTotal},{p.Estado}");
        }

        return File(System.Text.Encoding.UTF8.GetPreamble().Concat(System.Text.Encoding.UTF8.GetBytes(csv.ToString())).ToArray(),
            "text/csv", $"pedidos_{DateTime.Now:yyyyMMdd}.csv");
    }
}
