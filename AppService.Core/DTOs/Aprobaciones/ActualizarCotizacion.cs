using System;

namespace AppService.Core.DTOs.Aprobaciones;

public class ActualizarCotizacion
{
    public string UsuarioConectado { get; set; }
    public string IdCliente { get; set; }
    public string Cotizacion { get; set; }
    public int Renglon { get; set; }
    public int Propuesta { get; set; }
    
    public decimal TasaExcepcion { get; set; }
    public DateTime? FechaPago { get; set; }
    public bool ImprimirFacturaEnUSD { get; set; }
    public int SolicitudDeCredito { get; set; }
    
    
}