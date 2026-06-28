using System;
using System.Collections.Generic;

namespace AquaxWeb.Models
{
    public class DashboardClienteViewModel
    {
        public Cliente Cliente { get; set; } = new Cliente();
        
        public int ProductosEnCamino { get; set; }
        
        public DateTime? ProximaEntrega { get; set; }
        
        public decimal SaldoMonetario { get; set; }
        
        public int PuntosAqua { get; set; }
        
        public List<Pedido> ActividadReciente { get; set; } = new List<Pedido>();
        
        public Pedido? PedidoActivo { get; set; }
    }
}
