using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Administrador,Trabajador")]
public class CajaController : Controller
{
    private readonly AquaxDbContext _context;

    public CajaController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hoy = DateTime.Today;
        
        // Obtener todos los pedidos entregados hoy
        var esAdmin = User.IsInRole("Administrador");
        var pedidosHoyQuery = _context.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.MetodoPago)
            .Include(p => p.Cliente)
            .Where(p => p.FechaEntrega != null && p.FechaEntrega.Value.Date == hoy && p.Estado == "Entregado");

        if (!esAdmin)
        {
            var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);
            pedidosHoyQuery = pedidosHoyQuery.Where(p => p.UsuarioId == usuarioId);
        }

        var pedidosHoy = await pedidosHoyQuery.ToListAsync();

        ViewBag.EsAdmin = esAdmin;

        // Agrupar por repartidor
        var reportePorRepartidor = pedidosHoy
            .Where(p => p.Usuario != null)
            .GroupBy(p => p.Usuario!)
            .Select(g => new ReporteCajaViewModel
            {
                RepartidorId = g.Key.Id,
                RepartidorNombre = g.Key.Nombre,
                PedidosEntregados = g.Count(),
                EnvasesEntregados = g.Sum(p => p.EnvasesEntregados),
                EnvasesRecibidos = g.Sum(p => p.EnvasesRecibidos),
                TotalEfectivo = g.Where(p => p.MetodoPago?.Nombre == "Efectivo").Sum(p => p.MontoTotal),
                TotalTransferencias = g.Where(p => p.MetodoPago?.Nombre != "Efectivo").Sum(p => p.MontoTotal),
                TotalRecaudado = g.Sum(p => p.MontoTotal)
            }).ToList();

        // Totales globales
        ViewBag.TotalPedidos = reportePorRepartidor.Sum(r => r.PedidosEntregados);
        ViewBag.TotalEfectivo = reportePorRepartidor.Sum(r => r.TotalEfectivo);
        ViewBag.TotalTransferencias = reportePorRepartidor.Sum(r => r.TotalTransferencias);
        ViewBag.GranTotal = reportePorRepartidor.Sum(r => r.TotalRecaudado);

        return View(reportePorRepartidor);
    }
}

public class ReporteCajaViewModel
{
    public int RepartidorId { get; set; }
    public string RepartidorNombre { get; set; } = string.Empty;
    public int PedidosEntregados { get; set; }
    public int EnvasesEntregados { get; set; }
    public int EnvasesRecibidos { get; set; }
    public decimal TotalEfectivo { get; set; }
    public decimal TotalTransferencias { get; set; }
    public decimal TotalRecaudado { get; set; }
}
