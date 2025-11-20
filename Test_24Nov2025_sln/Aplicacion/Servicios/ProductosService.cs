using Aplicacion.Interfaces;
using Dominio.Productos;
using Contratos.General;
using Contratos.Productos;

namespace Aplicacion.Servicios;

public class ProductosService : IProductosService
{
    // OBJETIVO: Gestionar la lógica de negocio en los productos 
    
    private readonly IProductoRepository _repo;
    //private readonly ILogger<ProductosService> _logger;
    
    public ProductosService(IProductoRepository repo/*, ILogger<ProductosService> logger*/)
    {
        _repo = repo;
        //_logger = logger;
    }
    
    public async Task<PaginadoDto<ProductoDto>> ObtenerPaginado(int pagina, int tamanioPagina)
    {
        // 1. Obtener el total de registros
        var totalRegistros = await _repository.ContarTodos();
        
        // 2. Calcular el salto
        var saltar = (pagina - 1) * tamanioPagina;
        
        // 3. Obtener los items de la página
        var productos = await _repository.ObtenerPaginado(saltar, tamanioPagina);
        
        // 4. Mapear a DTOs
        var productosDto = productos.Select(p => new ProductoDto
        {
            Id = p.idpro,
            Nombre = p.producto,
            Precio = p.precio ?? 0
        }).ToList();
        
        // 5. Crear respuesta paginada
        return new PaginadoDto<ProductoDto>(
            productosDto,
            totalRegistros,
            pagina,
            tamanioPagina
        );
    }
}