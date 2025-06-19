using System;

namespace AppService.Core.Entities;

public class AppQuotesPaginate
{
    public int Id { get; set; }
    public string UsuarioConsulta { get; set; }
    public int AppGeneralQuotesId { get; set; }
    public string Cotizacion { get; set; }
    public string IdCliente { get; set; }
    public string RazonSocial { get; set; }
    public string Rif { get; set; }
    public int IdStatus { get; set; }
    public string Estatus { get; set; }
    public DateTime Fecha { get; set; }
  
    
    public string Items { get; set; }
    public string ClaseCss { get; set; }
    public int TotalRegistros { get; set; }
    
    public short CondicionPago { get; set; }
    public long IdContacto { get; set; }
    public string OrdenCompra { get; set; }
    public string Observaciones { get; set; }
    public Decimal IdDireccionFacturar { get; set; }
    public Decimal IdDireccionEntrega { get; set; }
    public long IdMoneda { get; set; }
    public bool FijarPrecioBs { get; set; }
    public string Direccion { get; set; }
    public Decimal IdMunicipio { get; set; }
    public string DescripcionMunicipio { get; set; }
    public string NombreContacto { get; set; }
   
 
    
}