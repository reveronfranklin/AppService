namespace AppService.Core.DTOs.Comisiones.PcOrdenesSinCalculoComision;

public class PcTipoPagoOrdenNoCalcularComisionUpdateDto
{
    public int Id { get; set; }
    public long Orden { get; set; }
    public int TipoPagoId { get; set; }
    
}