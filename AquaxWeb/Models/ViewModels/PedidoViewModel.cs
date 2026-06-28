namespace AquaxWeb.Models.ViewModels;

public class PedidoViewModel
{
    public List<Pedido> Pedidos { get; set; } = new();
    public List<Usuario> Trabajadores { get; set; } = new();
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public string? EstadoFiltro { get; set; }
    public bool EsAdmin { get; set; }
}
