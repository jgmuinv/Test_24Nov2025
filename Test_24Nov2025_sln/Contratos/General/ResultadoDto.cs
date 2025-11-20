namespace Contratos.General;

public record ResultadoDto<T>
{
    public bool Exitoso { get; init; }
    public T? Datos { get; init; }
    public string? Mensaje { get; init; }
    public List<string> Errores { get; init; } = new();
}