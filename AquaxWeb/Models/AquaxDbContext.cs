using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace AquaxWeb.Models;

public class AquaxDbContext : DbContext
{
    private readonly IConfiguration _configuration;

    public AquaxDbContext(DbContextOptions<AquaxDbContext> options, IConfiguration configuration) : base(options) 
    { 
        _configuration = configuration;
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<DetallePedido> DetallesPedido { get; set; }
    public DbSet<HistorialAccesibilidad> HistorialAccesibilidad { get; set; }
    public DbSet<HistorialSesion> HistorialSesiones { get; set; }
    public DbSet<MetodoPago> MetodosPago { get; set; }
    public DbSet<DireccionCliente> DireccionesClientes { get; set; }
    public DbSet<Gasto> Gastos { get; set; }
    public DbSet<MovimientoEnvase> MovimientosEnvases { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Correo)
            .IsUnique();

        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Correo)
            .IsUnique()
            .HasFilter("[Correo] != ''");

        modelBuilder.Entity<HistorialAccesibilidad>()
            .HasIndex(h => h.UsuarioId)
            .IsUnique();

        modelBuilder.Entity<DetallePedido>()
            .HasOne(dp => dp.Pedido)
            .WithMany(p => p.DetallesPedido)
            .HasForeignKey(dp => dp.PedidoId);

        modelBuilder.Entity<DetallePedido>()
            .HasOne(dp => dp.Producto)
            .WithMany(p => p.DetallesPedido)
            .HasForeignKey(dp => dp.ProductoId);

        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Cliente)
            .WithMany(c => c.Pedidos)
            .HasForeignKey(p => p.ClienteId);

        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Pedidos)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<HistorialAccesibilidad>()
            .HasOne(h => h.Usuario)
            .WithOne(u => u.HistorialAccesibilidad)
            .HasForeignKey<HistorialAccesibilidad>(h => h.UsuarioId);

        modelBuilder.Entity<HistorialSesion>()
            .HasOne(hs => hs.Usuario)
            .WithMany(u => u.HistorialSesiones)
            .HasForeignKey(hs => hs.UsuarioId);

        modelBuilder.Entity<DireccionCliente>()
            .HasOne(dc => dc.Cliente)
            .WithMany(c => c.Direcciones)
            .HasForeignKey(dc => dc.ClienteId);

        modelBuilder.Entity<MovimientoEnvase>()
            .HasOne(me => me.Cliente)
            .WithMany()
            .HasForeignKey(me => me.ClienteId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<MovimientoEnvase>()
            .HasOne(me => me.Pedido)
            .WithMany()
            .HasForeignKey(me => me.PedidoId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Pedido>()
            .HasOne(p => p.MetodoPago)
            .WithMany(mp => mp.Pedidos)
            .HasForeignKey(p => p.MetodoPagoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.Property(e => e.Monto).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.Property(e => e.Precio).HasPrecision(18, 2);
        });

        modelBuilder.Entity<Pedido>(entity =>
        {
            entity.Property(e => e.MontoTotal).HasPrecision(18, 2);
            entity.HasIndex(e => e.CodigoRastreo).IsUnique();
        });

        modelBuilder.Entity<DetallePedido>(entity =>
        {
            entity.Property(e => e.PrecioUnitario).HasPrecision(18, 2);
        });
    }
}
