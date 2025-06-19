using System;

namespace AppService.Core.Entities;

public class AppCalculadora
{
    public int Id { get; set; }
    public string IdVendedor { get; set; }
    public int IdMunicipio { get; set; }
    public string DescripcionMunicipio { get; set; }
    public int IdCondPago { get; set; }
    public string DescripcionCondPago { get; set; }
    public int IdMtrTipoMonedaPago { get; set; }
    
    public string DescripcionTipoMonedaPago{ get; set; }
    public int IdSubcategoria { get; set; }
    public string DescripcionSubcategoria { get; set; }
    
    public int IdProducto { get; set; }
    public string DescripcionProducto { get; set; }
    
    public Decimal Cantidad { get; set; }
    public int IdUnidad { get; set; }
    public string DescripcionUnidad { get; set; }
    public Decimal Cantidad_X_Unidad { get; set; }
    public Decimal CantidadConvertida { get; set; }
    public string MedidaBasica { get; set; }
    public string MedidaOpuesta { get; set; }
    public Decimal PrecioMinimo { get; set; }
    public Decimal PrecioMaximo { get; set; }
    public Decimal Precio { get; set; }
    public Decimal Total { get; set; }
    public int? CalculoId { get; set; }
    
    public string UserCreate { get; set; }

    public string UserUpdate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public string SearchText { get; set; }
    public string IdCliente { get; set; }
    public string RazonSocial { get; set; }
    public decimal PorcFlete { get; set; }
    
    
    public int IdDireccionEntregar { get; set; }
}