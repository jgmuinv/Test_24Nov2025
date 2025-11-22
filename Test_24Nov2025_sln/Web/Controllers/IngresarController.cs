using System.Net.Http;
using System.Net.Http.Json;
using Contratos.General;
using Contratos.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers;

public class IngresarController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IngresarController> _logger;

    public IngresarController(HttpClient httpClient, ILogger<IngresarController> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    [HttpGet]
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
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            // Ajusta la ruta según tu API:
            // si tu API usa [Route("[controller]/[action]")]
            // y el controlador se llama LoginController con acción Ingresar,
            // esta ruta "Login/Ingresar" es la correcta.
            var response = await _httpClient.PostAsJsonAsync("Auth/Login", model);

            if (!response.IsSuccessStatusCode)
            {
                var raw = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty,
                    $"No se pudo iniciar sesión. Detalle técnico: {raw}");
                return View(model);
            }

            // No nos interesa tanto el tipo de Datos, solo si fue exitoso y los errores
            var resultado = await response.Content
                .ReadFromJsonAsync<ResultadoDto<object>>();

            if (resultado == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Respuesta inválida del servidor al intentar iniciar sesión.");
                return View(model);
            }

            if (!resultado.Exitoso)
            {
                // Errores de dominio devueltos por el API
                foreach (var error in resultado.Errores)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model);
            }

            // Aquí normalmente guardarías el token/cookies, etc.
            // Por ahora solo redirigimos a Home/Index
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl)
                && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException ex)
        {
            // Error técnico (API caída, DNS, etc.)
            _logger.LogError(ex, "Error de comunicación con el API al intentar iniciar sesión.");
            ModelState.AddModelError(string.Empty,
                "No se pudo comunicar con el servidor de autenticación. Intente nuevamente más tarde.");
            return View(model);
        }
        catch (Exception ex)
        {
            // Error inesperado
            _logger.LogError(ex, "Error inesperado al intentar iniciar sesión.");
            ModelState.AddModelError(string.Empty,
                "Ocurrió un error inesperado al intentar iniciar sesión.");
            return View(model);
        }
    }
}