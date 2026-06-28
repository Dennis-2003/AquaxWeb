using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;
using System.Security.Claims;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Administrador,Trabajador")]
public class DespachoController : Controller
{
    private readonly AquaxDbContext _context;

    public DespachoController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var esAdmin = User.IsInRole("Administrador");
        var query = _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Usuario)
            .AsQueryable();

        if (!esAdmin)
        {
            var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);
            query = query.Where(p => p.UsuarioId == usuarioId);
        }

        var pedidos = await query
            .OrderByDescending(p => p.FechaPedido)
            .ToListAsync();

        ViewBag.EsAdmin = esAdmin;

        // Repartidores disponibles para asignar rutas
        ViewBag.Repartidores = await _context.Usuarios
            .Where(u => u.Rol == "Trabajador" || u.Rol == "Administrador")
            .ToListAsync();

        return View(pedidos);
    }

    [HttpPost]
    public async Task<IActionResult> AsignarRepartidor(int pedidoId, int usuarioId)
    {
        var pedido = await _context.Pedidos.FindAsync(pedidoId);
        if (pedido == null) return NotFound();

        pedido.UsuarioId = usuarioId;
        pedido.Estado = "EnCamino";
        
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Pedido asignado y en ruta.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> CompletarEntrega(int pedidoId, int envasesEntregados, int envasesRecibidos, string? observacion)
    {
        var pedido = await _context.Pedidos.Include(p => p.Cliente).FirstOrDefaultAsync(p => p.Id == pedidoId);
        if (pedido == null) return NotFound();

        // Evitar doble confirmación
        if (pedido.Estado == "Entregado") return RedirectToAction(nameof(Index));

        pedido.Estado = "Entregado";
        pedido.FechaEntrega = DateTime.Now;
        pedido.EnvasesEntregados = envasesEntregados;
        pedido.EnvasesRecibidos = envasesRecibidos;

        // Calcular la deuda de envases para este cliente
        // Si entrego 5 llenos y me devuelven 3 vacíos, el cliente debe 2.
        int diferencia = envasesEntregados - envasesRecibidos;
        pedido.Cliente.SaldoEnvases += diferencia;

        // Registrar el movimiento de envases
        var movimiento = new MovimientoEnvase
        {
            ClienteId = pedido.ClienteId,
            PedidoId = pedido.Id,
            Fecha = DateTime.Now,
            EnvasesEntregados = envasesEntregados,
            EnvasesDevueltos = envasesRecibidos,
            SaldoResultante = pedido.Cliente.SaldoEnvases,
            Observacion = observacion
        };
        _context.MovimientosEnvases.Add(movimiento);

        await _context.SaveChangesAsync();

        TempData["Mensaje"] = $"Entrega completada. Saldo de envases actualizado ({pedido.Cliente.SaldoEnvases} en poder del cliente).";
        return RedirectToAction(nameof(Index));
    }
}
