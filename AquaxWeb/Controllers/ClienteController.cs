using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;

namespace AquaxWeb.Controllers;

[Authorize(Roles = "Cliente")]
public class ClienteController : Controller
{
    private readonly AquaxDbContext _context;

    public ClienteController(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var clienteIdClaim = User.FindFirst("ClienteId")?.Value;
        if (clienteIdClaim == null) return RedirectToAction("Login", "Account");
        
        int clienteId = int.Parse(clienteIdClaim);
        
        var cliente = await _context.Clientes
            .Include(c => c.Pedidos)
            .ThenInclude(p => p.DetallesPedido)
            .ThenInclude(dp => dp.Producto)
            .Include(c => c.Pedidos)
            .ThenInclude(p => p.Usuario) // Repartidor
            .FirstOrDefaultAsync(c => c.Id == clienteId);
            
        if (cliente == null) return RedirectToAction("Login", "Account");

        var viewModel = new DashboardClienteViewModel
        {
            Cliente = cliente,
            SaldoMonetario = cliente.SaldoMonetario,
            PuntosAqua = cliente.PuntosAqua,
            
            // Productos en camino: sumamos cantidades de detalles donde el pedido esté en ruta
            ProductosEnCamino = cliente.Pedidos
                .Where(p => p.Estado == "En Ruta" || p.Estado == "En Tránsito")
                .SelectMany(p => p.DetallesPedido)
                .Sum(dp => dp.Cantidad),
                
            // Próxima entrega: fecha del pedido pendiente o en ruta más antiguo (que ya debería llegar) o más próximo
            ProximaEntrega = cliente.Pedidos
                .Where(p => p.Estado == "Pendiente" || p.Estado == "En Ruta")
                .OrderBy(p => p.FechaPedido)
                .FirstOrDefault()?.FechaPedido,
                
            // Actividad Reciente: últimos 5 pedidos
            ActividadReciente = cliente.Pedidos
                .OrderByDescending(p => p.FechaPedido)
                .Take(5)
                .ToList(),
                
            // Pedido Activo para el mapa de rastreo
            PedidoActivo = cliente.Pedidos
                .Where(p => p.Estado == "Pendiente" || p.Estado == "En Ruta" || p.Estado == "En Tránsito")
                .OrderBy(p => p.FechaPedido)
                .FirstOrDefault()
        };

        return View(viewModel);
    }
}
