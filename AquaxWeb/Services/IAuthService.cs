using AquaxWeb.Models;

namespace AquaxWeb.Services;

public interface IAuthService
{
    Task<Usuario?> ValidateUserAsync(string correo, string contrasena);
    Task<Cliente?> ValidateClientAsync(string correo, string contrasena);
}
