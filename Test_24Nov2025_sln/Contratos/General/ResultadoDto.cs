namespace Contratos.General;

public record ResultadoDto<T>
{
    public bool Exitoso { get; init; }
    public T? Datos { get; init; }
    public List<string> Errores { get; init; } = new();
    
    public static ResultadoDto<T> Success(T datos)
        => new() { Exitoso = true, Datos = datos };

    public static ResultadoDto<T> Failure(string error)
        => new() { Exitoso = false, Errores = new List<string> { error } };

    public static ResultadoDto<T> Failure(List<string> errores)
        => new() { Exitoso = false, Errores = errores };
}