using Microsoft.EntityFrameworkCore;

namespace AquaxWeb.Models;

public static class DbInitializer
{
    public static async Task SeedAsync(AquaxDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Usuarios.AnyAsync())
            return;

        context.Usuarios.Add(new Usuario
        {
            Nombre = "Imer sangay",
            Correo = "imersangay@aquax.com",
            Contrasena = "admin123",
            Rol = "Administrador",
            FechaRegistro = DateTime.Now
        });

        context.Usuarios.Add(new Usuario
        {
            Nombre = "Juan Pérez",
            Correo = "juan@aquax.com",
            Contrasena = "123456",
            Rol = "Trabajador",
            FechaRegistro = DateTime.Now
        });

        context.Productos.AddRange(
            new Producto { Nombre = "Agua Purificada", Formato = "Botella 500ml", Precio = 10, Stock = 100 },
            new Producto { Nombre = "Agua Purificada", Formato = "Botella 1L", Precio = 15, Stock = 80 },
            new Producto { Nombre = "Agua Purificada", Formato = "Garrafón 20L", Precio = 40, Stock = 50 }
        );

        await context.SaveChangesAsync();
    }
}
