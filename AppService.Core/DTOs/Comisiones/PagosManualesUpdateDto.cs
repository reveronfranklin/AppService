namespace AppService.Core.DTOs.Comisiones;

public class PagosManualesUpdateDto
{
    public long IdPago { get; set; }
    public string Transaccion { get; set; }
    public long Orden { get; set; }
    public string Producto { get; set; }
    public decimal Monto { get; set; }
    public decimal MontoGte { get; set; }
    public decimal MontoGteProducto { get; set; }
    public string Observaciones { get; set; }
    public string Usuario { get; set; }
}