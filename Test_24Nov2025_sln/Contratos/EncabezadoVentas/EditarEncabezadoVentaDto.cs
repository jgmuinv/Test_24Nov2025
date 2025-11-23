using System.ComponentModel.DataAnnotations;

namespace Contratos.EncabezadoVentas;

public record EditarEncabezadoVentaDto
{
    [Required(ErrorMessage = "El Id es obligatorio")]
    public int Idventa { get; set; }
    
    [Required(ErrorMessage = "La fecha es obligatoria")]
    public DateTime Fecha { get; set; } = DateTime.Now;
    
    [Required(ErrorMessage = "El vendedor es obligatorio")]
    public int Idvendedor { get; set; }
    
    [Required(ErrorMessage = "El total es obligatorio")]
    [Range(0.01, 99999999.99, ErrorMessage = "El precio debe estar entre 0.01 y 99,999,999.99")]
    public decimal Total { get; set; }
}