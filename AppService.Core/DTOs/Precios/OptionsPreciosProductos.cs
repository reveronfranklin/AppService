using System;

namespace AppService.Core.DTOs.Precios;

public class OptionsPreciosProductos
{
    public AppProductsGetDto Producto { get; set; }
    public Decimal Cantidad { get; set; }
    public decimal MedidaBasica { get; set; }
    public decimal MedidaOpuesta { get; set; }
    public int IdUnidad { get; set; }
    public MunicipioGetDto Municipio { get; set; }
    public int CondicionPago { get; set; }
}