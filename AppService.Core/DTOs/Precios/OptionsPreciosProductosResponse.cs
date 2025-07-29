namespace AppService.Core.DTOs.Precios;

public class OptionsPreciosProductosResponse
{
    public AppProductsGetDto Producto { get; set; }
    public decimal Cantidad { get; set; }
    public decimal PrecioMinimo { get; set; }
    public decimal PrecioMaximo { get; set; }
    public decimal CantidadConvertida { get; set; }
    public int IdCalculo { get; set; }
    public decimal Flete { get; set; }
    public decimal FleteMaximo { get; set; }
    public bool PorDebajoDeCantidadMinima { get; set; }
    
}