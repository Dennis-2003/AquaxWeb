using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AquaxWeb.Models;
using AquaxWeb.Services;

namespace AquaxWeb.Controllers;

public class AccountController : Controller
{
    private readonly IAuthService _authService;
    private readonly AquaxDbContext _context;

    public AccountController(IAuthService authService, AquaxDbContext context)
    {
        _authService = authService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (User.IsInRole("Cliente"))
                return RedirectToAction("Index", "Cliente");
            return RedirectToAction("Index", "Dashboard");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string correo, string contrasena)
    {
        var usuario = await _authService.ValidateUserAsync(correo, contrasena);

        if (usuario != null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, usuario.Nombre),
                new(ClaimTypes.Email, usuario.Correo),
                new(ClaimTypes.Role, usuario.Rol),
                new("UsuarioId", usuario.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Dashboard");
        }

        var cliente = await _authService.ValidateClientAsync(correo, contrasena);

        if (cliente != null)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, cliente.Nombre),
                new(ClaimTypes.Email, cliente.Correo),
                new(ClaimTypes.Role, "Cliente"),
                new("ClienteId", cliente.Id.ToString()),
                new("TipoCliente", cliente.TipoCliente)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            return RedirectToAction("Index", "Cliente");
        }

        ModelState.AddModelError("", "Correo o contraseña inválidos.");
        return View();
    }

    [HttpGet]
    public IActionResult RegistroCliente()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Cliente");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RegistroCliente(string nombre, string telefono, string direccion, string correo, string contrasena, string tipoCliente)
    {
        if (await _context.Clientes.AnyAsync(c => c.Correo == correo))
        {
            ModelState.AddModelError("", "Ese correo ya está registrado.");
            return View();
        }

        var cliente = new Cliente
        {
            Nombre = nombre,
            Telefono = telefono,
            Direccion = direccion,
            Correo = correo,
            Contrasena = contrasena,
            TipoCliente = tipoCliente == "Empresa" ? "Empresa" : "Natural",
            FechaRegistro = DateTime.Now
        };
        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        // Vincular pedidos previos hechos con ese teléfono
        var pedidosPrevios = await _context.Pedidos
            .Where(p => p.Cliente.Telefono == telefono && p.Cliente.Correo == null)
            .ToListAsync();
        foreach (var p in pedidosPrevios)
        {
            p.ClienteId = cliente.Id;
        }
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = "Cuenta creada con éxito. Ahora puedes iniciar sesión.";
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}