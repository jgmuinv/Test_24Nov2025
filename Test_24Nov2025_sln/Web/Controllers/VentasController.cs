using System.Collections;
using System.Net.Http;
using System.Net.Http.Json;
using System.Web;
using Contratos.General;
using Contratos.Productos;
using Contratos.EncabezadoVentas;
using Contratos.DetalleVentas;
using Contratos.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        var respNombresUsuarios = await ListaNombreUsuarios();
        obj.ListaUsuarios = respNombresUsuarios.Datos;
        
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
            var respNombreUsuarios = await ListaNombreUsuarios();
            obj.ListaUsuarios = respNombreUsuarios.Datos;
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
                TempData["Mensaje"] = "No se pudo obtener el detalle del registro.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<IReadOnlyList<EncabezadoVentaDto?>>>();

            if (resultado == null || !resultado.Exitoso || resultado.Datos == null)
            {
                TempData["Mensaje"] = string.Join(" ", resultado?.Errores ?? new List<string> { "No se encontró el registro." });
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var dto = resultado.Datos.FirstOrDefault();
            if (dto == null)
            {
                TempData["Mensaje"] = "No se encontró el registro.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }
            
            // var vm = new EncabezadoVentaDto
            // {
            //     Idventa = dto.Id,
            //     Total = dto.
            // };

            return View(dto);
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
    // GET: /Ventas/Create
    // Mostrar formulario de creación
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var obj = new VentasCrearEncabezadoViewModel();
        var respNombresUsuarios = await ListaNombreUsuarios();
        obj.ListaUsuarios = respNombresUsuarios.Datos;
        
        return View(obj);
    }

    // =========================================================
    // POST: /Ventas/Create
    // Enviar formulario al endpoint /Ventas/Crear del API
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VentasCrearEncabezadoViewModel model)
    {
        var respNombresUsuarios = await ListaNombreUsuarios();
        model.ListaUsuarios = respNombresUsuarios.Datos;
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync("EncabezadoVentas/Crear", model);

            if (!response.IsSuccessStatusCode)
            {
                var cuerpo = await response.Content.ReadAsStringAsync();
                model.Mensaje = $"No se pudo crear el registro. Detalle técnico: {cuerpo}";
                model.TipoMensaje = "danger";
                return View(model);
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<ProductoDto?>>();

            if (resultado == null)
            {
                model.Mensaje = "Respuesta inválida del servidor al crear el registro.";
                model.TipoMensaje = "danger";
                return View(model);
            }

            if (!resultado.Exitoso)
            {
                model.Mensaje = string.Join(", ", resultado.Errores);
                model.TipoMensaje = "danger";

                return View(model); // Errores de dominio mostrados al usuario
            }

            TempData["Mensaje"] = "Registro creado correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al crear registro");
            ModelState.AddModelError(string.Empty, "No se pudo comunicar con el servidor para crear el registro.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear registro");
            ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al crear el registro.");
            return View(model);
        }
    }

    // =========================================================
    // GET: /Ventas/Edit/5
    // Cargar datos llamando a /EncabezadoVentas/Listar?idpro={id}
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"EncabezadoVentas/Listar?idventa={id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "No se pudo obtener el registro para editar.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<IReadOnlyList<EncabezadoVentaDto?>>>();

            if (resultado == null || !resultado.Exitoso || resultado.Datos == null)
            {
                TempData["Mensaje"] = string.Join(" ", resultado?.Errores ?? new List<string> { "No se encontró el registro." });
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var entidad = resultado.Datos.FirstOrDefault();
            if (entidad == null)
            {
                TempData["Mensaje"] = "No se encontró el registro.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var model = new VentasEditarEncabezadoViewModel()
            {
                Idventa = entidad.Idventa,
                Idvendedor = entidad.Idvendedor,
                Total = entidad.Total,
                DetalleVenta = entidad.DetalleVenta,
                Fecha = entidad.Fecha,
                NombreVendedor = entidad.NombreVendedor
            };

            return View(model);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al obtener registro {Id} para edición", id);
            TempData["Mensaje"] = "No se pudo comunicar con el servidor para obtener el registro.";
            TempData["TipoMensaje"] = "danger";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener registro {Id} para edición", id);
            TempData["Mensaje"] = "Ocurrió un error inesperado al obtener el registro.";
            TempData["TipoMensaje"] = "danger";
            return RedirectToAction(nameof(Index));
        }
    }

    // =========================================================
    // POST: /Ventas/Edit/5
    // Enviar formulario al endpoint /Ventas/Actualizar/{id} del API
    // =========================================================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, VentasEditarEncabezadoViewModel model)
    {
        if (id != model.Idventa)
        {
            ModelState.AddModelError(string.Empty, "El identificador del registro no coincide.");
            return View(model);
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            if (model.NuevoDvIdPro <= 0)
            {
                model.Mensaje = "Debe ingresar el código del producto.";
                model.TipoMensaje = "danger";
                return View(model);
            }
            
            if (string.IsNullOrEmpty(model.NuevoDvProducto))
            {
                model.Mensaje = "Debe ingresar el nombre del producto.";
                model.TipoMensaje = "danger";
                return View(model);
            }
            
            if (model.NuevoDvPrecio <= 0)
            {
                model.Mensaje = "Debe ingresar el precio del producto.";
                model.TipoMensaje = "danger";
                return View(model);
            }
            
            if (model.NuevoDvCantidad <= 0)
            {
                model.Mensaje = "Debe ingresar la cantidad del producto.";
                model.TipoMensaje = "danger";
                return View(model);
            }

            var reqDetalleVentaNuevo = new CrearDetalleVentaDto
            {
                Idventa = model.Idventa,
                Idpro = model.NuevoDvIdPro,
                Cantidad = model.NuevoDvCantidad,
                Precio = model.NuevoDvPrecio
            };
            
            var responseDetalle = await _httpClient.PostAsJsonAsync($"DetalleVentas/Crear", reqDetalleVentaNuevo);
            
            if (!responseDetalle.IsSuccessStatusCode)
            {
                var cuerpo = await responseDetalle.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"No se pudo actualizar el registro. Detalle técnico: {cuerpo}");
                return View(model);
            }

            var resultadoDetalle =
                await responseDetalle.Content.ReadFromJsonAsync<ResultadoDto<DetalleVentaDto?>>();

            if (resultadoDetalle == null)
            {
                ModelState.AddModelError(string.Empty, "Respuesta inválida del servidor al actualizar el registro.");
                return View(model);
            }

            if (!resultadoDetalle.Exitoso)
            {
                foreach (var error in resultadoDetalle.Errores)
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                return View(model); // errores de dominio
            }
            
            TempData["Mensaje"] = "Detalle agregado correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Edit), new { id = model.Idventa });
            
            //
            //
            //
            // var response = await _httpClient.PutAsJsonAsync($"EncabezadoVentas/Actualizar/{id}", model);
            //
            // if (!response.IsSuccessStatusCode)
            // {
            //     var cuerpo = await response.Content.ReadAsStringAsync();
            //     ModelState.AddModelError(string.Empty, $"No se pudo actualizar el registro. Detalle técnico: {cuerpo}");
            //     return View(model);
            // }
            //
            // var resultado =
            //     await response.Content.ReadFromJsonAsync<ResultadoDto<EncabezadoVentaDto?>>();
            //
            // if (resultado == null)
            // {
            //     ModelState.AddModelError(string.Empty, "Respuesta inválida del servidor al actualizar el registro.");
            //     return View(model);
            // }
            //
            // if (!resultado.Exitoso)
            // {
            //     foreach (var error in resultado.Errores)
            //     {
            //         ModelState.AddModelError(string.Empty, error);
            //     }
            //
            //     return View(model); // errores de dominio
            // }
            //
            // TempData["Mensaje"] = "Registro actualizado correctamente.";
            // TempData["TipoMensaje"] = "success";
            // return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al actualizar registro {Id}", id);
            ModelState.AddModelError(string.Empty, "No se pudo comunicar con el servidor para actualizar el registro.");
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar registro {Id}", id);
            ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado al actualizar el registro.");
            return View(model);
        }
    }

    // =========================================================
    // GET: /Ventas/Delete/5
    // Confirmación de eliminación
    // =========================================================
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"Ventas/Listar?idpro={id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "No se pudo obtener el registro para eliminar.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<IReadOnlyList<ProductoDto?>>>();

            if (resultado == null || !resultado.Exitoso || resultado.Datos == null)
            {
                TempData["Mensaje"] = string.Join(" ", resultado?.Errores ?? new List<string> { "No se encontró el registro." });
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Index));
            }

            var productoDto = resultado.Datos.FirstOrDefault();
            if (productoDto == null)
            {
                TempData["Error"] = "No se encontró el registro.";
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
            _logger.LogError(ex, "Error de comunicación con el API al obtener registro {Id} para eliminación", id);
            TempData["Error"] = "No se pudo comunicar con el servidor para obtener el registro.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener registro {Id} para eliminación", id);
            TempData["Error"] = "Ocurrió un error inesperado al obtener el registro.";
            return RedirectToAction(nameof(Index));
        }
    }

    // =========================================================
    // POST: /Ventas/Delete/5
    // Ejecutar eliminación llamando a /Ventas/Eliminar/{id}
    // =========================================================
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, int idventa)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"DetalleVentas/Eliminar/{id}");

            if (!response.IsSuccessStatusCode)
            {
                var cuerpo = await response.Content.ReadAsStringAsync();
                TempData["Mensaje"] = $"No se pudo eliminar el registro. Detalle técnico: {cuerpo}";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Edit), new { id = idventa });
            }

            var resultado =
                await response.Content.ReadFromJsonAsync<ResultadoDto<bool?>>();

            if (resultado == null)
            {
                TempData["Mensaje"] = "Respuesta inválida del servidor al eliminar el registro.";
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Edit), new { id = idventa });
            }

            if (!resultado.Exitoso)
            {
                TempData["Mensaje"] = string.Join(" ", resultado.Errores);
                TempData["TipoMensaje"] = "danger";
                return RedirectToAction(nameof(Edit), new { id = idventa });
            }

            TempData["Mensaje"] = "Registro eliminado correctamente.";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(Edit), new { id = idventa });
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error de comunicación con el API al eliminar registro {Id}", id);
            TempData["Mensaje"] = "No se pudo comunicar con el servidor para eliminar el registro.";
            TempData["TipoMensaje"] = "danger";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar registro {Id}", id);
            TempData["Mensaje"] = "Ocurrió un error inesperado al eliminar el registro.";
            TempData["TipoMensaje"] = "danger";
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task<ResultadoDto<IEnumerable<SelectListItem>> > ListaNombreUsuarios()
    {
        var url = $"Usuarios/ListarNombres";
        
        var response = await _httpClient.GetAsync(url);
        
        if (!response.IsSuccessStatusCode)
        {
            return ResultadoDto<IEnumerable<SelectListItem>>.Failure("No se pudieron obtener los registros de vendedores."); ;
        }

        var resultado =
            await response.Content.ReadFromJsonAsync<ResultadoDto<List<NombreUsuariosDto?>>>();

        if (resultado == null)
        {
            return ResultadoDto<IEnumerable<SelectListItem>>.Failure("Respuesta inválida del servidor al listar registros.");
        }

        if (!resultado.Exitoso)
        {
            return ResultadoDto<IEnumerable<SelectListItem>>.Failure("Respuesta inválida del servidor al listar registros.");
        }

        // Datos que devuelve la API
        var respApi = resultado.Datos ?? [];

        var salida = respApi.Select(u => new SelectListItem
        {
            Value = u.IdUs.ToString(),
            Text = u.Nombre
        });
        
        return ResultadoDto<IEnumerable<SelectListItem>>.Success(salida); ;
    }
}