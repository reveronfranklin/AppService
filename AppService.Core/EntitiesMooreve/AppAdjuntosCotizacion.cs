using System;

namespace AppService.Core.EntitiesMooreve;

public class AppAdjuntosCotizacion
{
    public int Id { get; set; }
    public string Cotizacion { get; set; }
    public int? Renglon { get; set; }
    public int? IdAdjunto { get; set; }
    public string NombreDocumento { get; set; }
    public byte[] Archivo { get; set; }
    public string NombreArchivo { get; set; }
    public DateTime? FechaCreacion { get; set; }
    
}