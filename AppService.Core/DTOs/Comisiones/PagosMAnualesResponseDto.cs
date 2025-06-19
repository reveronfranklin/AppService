namespace AppService.Core.DTOs.Comisiones;

public class PagosMAnualesResponseDto
{
    public long IdPago { get; set; }
    public string Transaccion { get; set; }
    public string Producto { get; set; }
    public string OrdenString { get; set; }
    public decimal Monto { get; set; }
    public decimal MontoGte { get; set; }
    public decimal MontoGteProducto { get; set; }
    public long FichaGteProducto { get; set; }
    public string Observaciones { get; set; }
    public string SearchText { get; set; }
}