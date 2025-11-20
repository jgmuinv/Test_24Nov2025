namespace Contratos.General;

public record PaginadoDto<T>
{
    public List<T> Items { get; init; } = new();
    public int PaginaActual { get; init; }
    public int TamanioPagina { get; init; }
    public int TotalRegistros { get; init; }
    public int TotalPaginas { get; init; }
    public bool TienePaginaAnterior => PaginaActual > 1;
    public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;
    // public int PrimeraPagina => 1;
    // public int UltimaPagina => TotalPaginas;
    // public int IndicePrimerElemento => TotalRegistros == 0 ? 0 : ((PaginaActual - 1) * TamanioPagina) + 1;
    // public int IndiceUltimoElemento => Math.Min(PaginaActual * TamanioPagina, TotalRegistros);
    
    // Constructor vacío para serialización
    public PaginadoDto() { }
    
    // Constructor con parámetros
    public PaginadoDto(List<T> items, int totalRegistros, int paginaActual, int tamanioPagina)
    {
        Items = items;
        TotalRegistros = totalRegistros;
        PaginaActual = paginaActual;
        TamanioPagina = tamanioPagina;
        TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanioPagina);
    }
    
    // Método factory para crear respuesta paginada vacía
    public static PaginadoDto<T> Vacio(int paginaActual, int tamanioPagina)
        => new(new List<T>(), 0, paginaActual, tamanioPagina);
}