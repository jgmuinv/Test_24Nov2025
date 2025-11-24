using System.Net.Http;
using System.Net.Http.Json;
using System.Web;
using Contratos.General;
using Contratos.Productos;
using Contratos.EncabezadoVentas;
using Contratos.DetalleVentas;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Web.Models;

namespace Web.Controllers;

public class VentasController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VentasController> _logger;
    
    public VentasController(IHttpClientFactory httpClientFactory, ILogger<VentasController> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ApiClient");
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var obj = new VentasFiltroPaginadoViewModel();
        // var url = $"EncabezadoVentas/ListarPaginado?{query}";
        return View(obj);
    }

    // =========================================================
    // POST: /Ventas
    // Lista paginada de registros (/EncabezadoVentas/ListarPaginado?idvendedor=&paginaActual=1&registrosPorPagina=1)
    // =========================================================
    [HttpPost]
    public async Task<IActionResult> Index(VentasFiltroPaginadoViewModel obj)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                obj.Mensaje = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).FirstOrDefault();
                return View(obj);
            }
            
            // Validación y construcción de querystring con los parámetros que exige el API
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (obj.IdVendedor.HasValue) query["idvendedor"] = obj.IdVendedor.Value.ToString();
            
            query["paginaActual"] = obj.Resultados.PaginaActual.ToString();
            query["registrosPorPagina"] = obj.Resultados.TamanioPagina.ToString();

            var url = $"EncabezadoVentas/ListarPaginado?{query}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudieron obtener los registros.";
                return View(new PaginadoDto<EncabezadoVentaDto?>
                {
                    Items = new List<EncabezadoVentaDto?>(),
                    PaginaActual = obj.Resultados.PaginaActual,
                    TamanioPagina = obj.Resultados.TamanioPagina,
                    TotalRegistros = 0,
                    TotalPaginas = 0
                });
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<PaginadoDto<EncabezadoVentaDto?>>>();

            if (resultado == null)
            {
                TempData["Error"] = "Respuesta inválida del servidor al listar registros.";
                return View(new PaginadoDto<EncabezadoVentaDto?>
                {
                    Items = new List<EncabezadoVentaDto?>(),
                    PaginaActual = obj.Resultados.PaginaActual,
                    TamanioPagina = obj.Resultados.TamanioPagina,
                    TotalRegistros = 0,
                    TotalPaginas = 0
                });
            }

            if (!resultado.Exitoso)
            {
                TempData["Error"] = string.Join(" ", resultado.Errores);
                return View(new PaginadoDto<EncabezadoVentaDto?>
                {
                    Items = new List<EncabezadoVentaDto?>(),
                    PaginaActual = obj.Resultados.PaginaActual,
                    TamanioPagina = obj.Resultados.TamanioPagina,
                    TotalRegistros = 0,
                    TotalPaginas = 0
                });
            }

            // Datos paginados que devuelve la API
            var paginado = resultado.Datos ?? new PaginadoDto<EncabezadoVentaDto?>
            {
                Items = new List<EncabezadoVentaDto?>(),
                PaginaActual = obj.Resultados.PaginaActual,
                TamanioPagina = obj.Resultados.TamanioPagina,
                TotalRegistros = 0,
                TotalPaginas = 0
            };

            obj.Resultados = paginado;
            obj.Mensaje = "Filtrado realizado correctamente.";
            obj.TipoMensaje = "success";
            return View(obj);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al listar registros paginados");
            TempData["Error"] = "No se pudo comunicar con el servidor para listar los registros.";
            return View(new PaginadoDto<EncabezadoVentaDto?>
            {
                Items = new List<EncabezadoVentaDto?>(),
                PaginaActual = obj.Resultados.PaginaActual,
                TamanioPagina = obj.Resultados.TamanioPagina,
                TotalRegistros = 0,
                TotalPaginas = 0
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al listar registros paginados");
            TempData["Error"] = "Ocurrió un error inesperado al listar los registros.";
            return View(new PaginadoDto<EncabezadoVentaDto?>
            {
                Items = new List<EncabezadoVentaDto?>(),
                PaginaActual = obj.Resultados.PaginaActual,
                TamanioPagina = obj.Resultados.TamanioPagina,
                TotalRegistros = 0,
                TotalPaginas = 0
            });
        }
    }

    // =========================================================
    // GET: /Ventas/Details/5
    // Usa /EncabezadoVentas/Listar?idventa={id} del API
    // =========================================================
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"EncabezadoVentas/Listar?idventa={id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo obtener el detalle del registro.";
                return RedirectToAction(nameof(Index));
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<IReadOnlyList<ProductoDto?>>>();

            if (resultado == null || !resultado.Exitoso || resultado.Datos == null)
            {
                TempData["Error"] = string.Join(" ", resultado?.Errores ?? new List<string> { "No se encontró el producto." });
                return RedirectToAction(nameof(Index));
            }

            var productoDto = resultado.Datos.FirstOrDefault();
            if (productoDto == null)
            {
                TempData["Error"] = "No se encontró el producto.";
                return RedirectToAction(nameof(Index));
            }

            var vm = new ProductoDto
            {
                Id = productoDto.Id,
                Nombre = productoDto.Nombre,
                Precio = productoDto.Precio
            };

            return View(vm);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al obtener detalle del producto {Id}", id);
            TempData["Error"] = "No se pudo comunicar con el servidor para obtener el detalle del producto.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener detalle del producto {Id}", id);
            TempData["Error"] = "Ocurrió un error inesperado al obtener el detalle del producto.";
            return RedirectToAction(nameof(Index));
        }
    }

    // =========================================================
    // GET: /Productos/Create
    // Mostrar formulario de creación
    // =========================================================
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CrearProductoRequest());
    }

    // =========================================================
    // POST: /Productos/Create
    // Enviar formulario al endpoint /Productos/Crear del API
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CrearProductoRequest model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync("Productos/Crear", model);

            if (!response.IsSuccessStatusCode)
            {
                var cuerpo = await response.Content.ReadAsStringAsync();
                model.Mensaje = $"No se pudo crear el producto. Detalle técnico: {cuerpo}";
                model.TipoMensaje = "danger";
                return View(model);
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<ProductoDto?>>();

            if (resultado == null)
            {
                model.Mensaje = "Respuesta inválida del servidor al crear el producto.";
                model.TipoMensaje = "danger";
                return View(model);
            }

            if (!resultado.Exitoso)
            {
                model.Mensaje = string.Join(", ", resultado.Errores);
                model.TipoMensaje = "danger";

                return View(model); // Errores de dominio mostrados al usuario
            }

            TempData["Mensaje"] = "Producto creado correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al crear producto");
            ModelState.AddModelError(string.Empty, "No se pudo comunicar con el servidor para crear el producto.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear producto");
            ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear el producto.");
            return View(model);
        }
    }

    // =========================================================
    // GET: /Productos/Edit/5
    // Cargar datos llamando a /Productos/Listar?idpro={id}
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Productos/Listar?idpro={id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "No se pudo obtener el producto para editar.";
                return RedirectToAction(nameof(Index));
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<IReadOnlyList<ProductoDto?>>>();

            if (resultado == null || !resultado.Exitoso || resultado.Datos == null)
            {
                TempData["Error"] = string.Join(" ", resultado?.Errores ?? new List<string> { "No se encontró el producto." });
                return RedirectToAction(nameof(Index));
            }

            var productoDto = resultado.Datos.FirstOrDefault();
            if (productoDto == null)
            {
                TempData["Error"] = "No se encontró el producto.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ActualizarProductoRequest
            {
                Id = productoDto.Id,
                Nombre = productoDto.Nombre,
                Precio = productoDto.Precio
            };

            return View(model);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al obtener producto {Id} para edición", id);
            TempData["Error"] = "No se pudo comunicar con el servidor para obtener el producto.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener producto {Id} para edición", id);
            TempData["Error"] = "Ocurrió un error inesperado al obtener el producto.";
            return RedirectToAction(nameof(Index));
        }
    }

    // =========================================================
    // POST: /Productos/Edit/5
    // Enviar formulario al endpoint /Productos/Actualizar/{id} del API
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ActualizarProductoRequest model)
    {
        if (id != model.Id)
        {
            ModelState.AddModelError(string.Empty, "El identificador del producto no coincide.");
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _httpClient.PutAsJsonAsync($"Productos/Actualizar/{id}", model);

            if (!response.IsSuccessStatusCode)
            {
                var cuerpo = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"No se pudo actualizar el producto. Detalle técnico: {cuerpo}");
                return View(model);
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<ProductoDto?>>();

            if (resultado == null)
            {
                ModelState.AddModelError(string.Empty, "Respuesta inválida del servidor al actualizar el producto.");
                return View(model);
            }

            if (!resultado.Exitoso)
            {
                foreach (var error in resultado.Errores)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model); // errores de dominio
            }

            TempData["Mensaje"] = "Producto actualizado correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al actualizar producto {Id}", id);
            ModelState.AddModelError(string.Empty, "No se pudo comunicar con el servidor para actualizar el producto.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar producto {Id}", id);
            ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar el producto.");
            return View(model);
        }
    }

    // =========================================================
    // GET: /Productos/Delete/5
    // Confirmación de eliminación
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Productos/Listar?idpro={id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "No se pudo obtener el producto para eliminar.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<IReadOnlyList<ProductoDto?>>>();

            if (resultado == null || !resultado.Exitoso || resultado.Datos == null)
            {
                TempData["Mensaje"] = string.Join(" ", resultado?.Errores ?? new List<string> { "No se encontró el producto." });
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var productoDto = resultado.Datos.FirstOrDefault();
            if (productoDto == null)
            {
                TempData["Error"] = "No se encontró el producto.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var vm = new ProductoDto
            {
                Id = productoDto.Id,
                Nombre = productoDto.Nombre,
                Precio = productoDto.Precio
            };

            return View(vm);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al obtener producto {Id} para eliminación", id);
            TempData["Error"] = "No se pudo comunicar con el servidor para obtener el producto.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener producto {Id} para eliminación", id);
            TempData["Error"] = "Ocurrió un error inesperado al obtener el producto.";
            return RedirectToAction(nameof(Index));
        }
    }

    // =========================================================
    // POST: /Productos/Delete/5
    // Ejecutar eliminación llamando a /Productos/Eliminar/{id}
    // =========================================================
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"Productos/Eliminar/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var cuerpo = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"No se pudo eliminar el producto. Detalle técnico: {cuerpo}";
                return RedirectToAction(nameof(Index));
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<bool?>>();

            if (resultado == null)
            {
                TempData["Error"] = "Respuesta inválida del servidor al eliminar el producto.";
                return RedirectToAction(nameof(Index));
            }

            if (!resultado.Exitoso)
            {
                TempData["Error"] = string.Join(" ", resultado.Errores);
                return RedirectToAction(nameof(Index));
            }

            TempData["Exito"] = "Producto eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al eliminar producto {Id}", id);
            TempData["Error"] = "No se pudo comunicar con el servidor para eliminar el producto.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar producto {Id}", id);
            TempData["Error"] = "Ocurrió un error inesperado al eliminar el producto.";
            return RedirectToAction(nameof(Index));
        }
    }
}