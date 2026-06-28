using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;
using AquaxWeb.Models.ViewModels;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Administrador,Trabajador")]
public class DashboardController : Controller
{
    private readonly AquaxDbContext _context;

    public DashboardController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hoy = DateTime.Today;

        var esAdmin = User.IsInRole("Administrador");

        var queryPedidos = _context.Pedidos.AsQueryable();
        
        if (!esAdmin)
        {
            var usuarioId = int.Parse(User.FindFirst("UsuarioId")!.Value);
            queryPedidos = queryPedidos.Where(p => p.UsuarioId == usuarioId);
        }

        var modelo = new DashboardViewModel
        {
            PedidosHoy = await queryPedidos.CountAsync(p => p.FechaPedido.Date == hoy),
            PedidosPendientes = await queryPedidos.CountAsync(p => p.Estado == "Pendiente" && p.UsuarioId != null),
            PedidosEnRuta = await queryPedidos.CountAsync(p => p.Estado == "En Ruta" || p.Estado == "EnCamino"),
            PedidosSinAsignar = esAdmin ? await _context.Pedidos.CountAsync(p => p.UsuarioId == null) : 0,
            TotalClientes = esAdmin ? await _context.Clientes.CountAsync() : 0,
            TotalProductos = esAdmin ? await _context.Productos.CountAsync() : 0,
            VentasHoy = await queryPedidos
                .Where(p => p.FechaPedido.Date == hoy && p.Estado == "Entregado")
                .SumAsync(p => (decimal?)p.MontoTotal) ?? 0,
            EsAdmin = esAdmin,
            Trabajadores = esAdmin
                ? await _context.Usuarios.Where(u => u.Rol == "Trabajador").ToListAsync()
                : new(),
            UltimosPedidos = await queryPedidos
                .Include(p => p.Cliente)
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.FechaPedido)
                .Take(5)
                .ToListAsync()
        };

        return View(modelo);
    }
}
