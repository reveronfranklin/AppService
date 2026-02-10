using System;
using System.Collections.Generic;
using System.Text;

namespace AppService.Core.DTOs
{
    public class StatusAprobacionDto
    {
        public StatusAprobacionDto(string cotizacion, string producto)
        {
            Cotizacion = cotizacion;
            Producto = producto;
        }
        //nvarchar(13)
        public string Cotizacion { get; set; }
        //nvarchar(12)
        public string Producto { get; set; }
        public bool? FlagAprobado { get; set; }
        public bool? FlagCerrado { get; set; }
        public decimal? ValorVentaAprobar { get; set; }
        public decimal? ValorVentaAprobarUsd { get; set; }
        public decimal? precioEstimacion { get; set; }
      

        public bool Aprobado { get; set; }
        public string Color { get; set; }
        public string StatusString { get; set; }

    }
}
