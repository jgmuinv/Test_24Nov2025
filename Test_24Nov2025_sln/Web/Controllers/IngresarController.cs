using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using Contratos.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers;

public class IngresarController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IngresarController> _logger;

    public IngresarController(IHttpClientFactory httpClientFactory, ILogger<IngresarController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginRequest { ReturnUrl = returnUrl });
    }

    // =========================================================
    // POST: /Ingresar/Login
    // Llama al endpoint de login del API
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync("Auth/Login", model);

            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty,
                    $"No se pudo iniciar sesión. Detalle técnico: {raw}");
                return View(model);
            }

            var resultado = await response.Content.ReadFromJsonAsync<LoginResponse>();

            if (resultado == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Respuesta inválida del servidor al intentar iniciar sesión.");
                return View(model);
            }

            if (!resultado.Ok)
            {
                ModelState.AddModelError(string.Empty,
                    resultado.Error ?? "Credenciales inválidas.");
                return View(model);
            }

            // ===== AQUÍ: crear cookie de autenticación =====

            var claims = new List<Claim>
            {
                // Nombre mostrado (del token o del request)
                new Claim(ClaimTypes.Name, resultado.Usuario ?? model.Usuario),
                // Nombre de usuario de login
                new Claim("Usuario", model.Usuario)
            };

            if (!string.IsNullOrWhiteSpace(resultado.Token))
            {
                // Guardar el JWT para usarlo luego con el HttpClient si quieres
                claims.Add(new Claim("JwtToken", resultado.Token));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // recordar sesión
                // ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)  // opcional
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties);

            // ================================================

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl)
                && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al intentar iniciar sesión.");
            ModelState.AddModelError(string.Empty,
                "No se pudo comunicar con el servidor de autenticación. Intente nuevamente más tarde.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al intentar iniciar sesión.");
            ModelState.AddModelError(string.Empty,
                "Ocurrió un error inesperado al intentar iniciar sesión.");
            return View(model);
        }
    }
}