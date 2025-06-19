using System;
using AppService.Core.Entities;

namespace AppService.Core.DTOs.AppCalculadora;

public class AppCalculadoraResponseDto
{
    public int Id { get; set; }
    public string Fecha { get; set; }
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
    
    public string IdCliente { get; set; }
    
    public int IdDireccionEntregar { get; set; }
    public string RazonSocial { get; set; }
    public string SearchText { get; set; }
    public Decimal PorcFlete { get; set; }
    public AppProductsGetDto AppProduct { get; set; }
    public AppProductConversionGetDto AppProductConversion { get; set; }
}