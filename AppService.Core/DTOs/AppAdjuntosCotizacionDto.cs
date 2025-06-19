using System;
using System.Collections.Generic;
using System.Text;

namespace AppService.Core.DTOs
{
    public class AppAdjuntosCotizacionDto
    {        
        public string Cotizacion { get; set; }
        public int Renglon { get; set; }
        
        public string NombreDocumento { get; set; }
        public string NombreArchivo { get; set; }
        public string Ruta { get; set; }    
        public string Link { get; set; }
        public DateTime FechaCreacion { get; set; }

    }
}
