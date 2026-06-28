namespace AquaxWeb.Models.ViewModels;

public class DashboardViewModel
{
    public int PedidosHoy { get; set; }
    public int PedidosPendientes { get; set; }
    public int PedidosEnRuta { get; set; }
    public int PedidosSinAsignar { get; set; }
    public int TotalClientes { get; set; }
    public int TotalProductos { get; set; }
    public decimal VentasHoy { get; set; }
    public bool EsAdmin { get; set; }
    public List<Usuario> Trabajadores { get; set; } = new();
    public List<Pedido> UltimosPedidos { get; set; } = new();
}
