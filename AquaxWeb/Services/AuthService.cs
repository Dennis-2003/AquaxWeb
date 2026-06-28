using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;

namespace AquaxWeb.Services;

public class AuthService : IAuthService
{
    private readonly AquaxDbContext _context;

    public AuthService(AquaxDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> ValidateUserAsync(string correo, string contrasena)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

        if (usuario == null || usuario.Contrasena != contrasena)
            return null;

        return usuario;
    }

    public async Task<Cliente?> ValidateClientAsync(string correo, string contrasena)
    {
        var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Correo == correo);

        if (cliente == null || cliente.Contrasena != contrasena)
            return null;

        return cliente;
    }
}
