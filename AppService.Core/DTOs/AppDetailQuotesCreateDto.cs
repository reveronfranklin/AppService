﻿using System;

namespace AppService.Core.DTOs
{
    public class AppDetailQuotesCreateDto
    {



        public int AppGeneralQuotesId { get; set; }
        public string Cotizacion { get; set; }
        public int IdProducto { get; set; }
        public string NombreComercialProducto { get; set; }
        public int IdEstatus { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadSolicitada { get; set; }
        public decimal Precio { get; set; }
        public decimal Total { get; set; }
        public decimal PrecioUsd { get; set; }
        public decimal TotalUsd { get; set; }
        public int IdUnidad { get; set; }
        public string Observaciones { get; set; }
        public int DiasEntrega { get; set; }
        public decimal PrecioLista { get; set; }

        public bool SolicitarPrecio { get; set; }
        public string ObsSolicitud { get; set; }


        public decimal ValorConvertido { get; set; }
        public decimal UnitPriceBaseProduction { get; set; }
        public decimal UnitPriceConverted { get; set; }
        public decimal CantidadPorUnidadProduccion { get; set; }
        public int QuantityPerPackage { get; set; }
        public long OrdenAnterior { get; set; }

        public Decimal MedidaBasica { get; set; }

        public Decimal MedidaOpuesta { get; set; }
        public int CalculoId { get; set; }

        public Decimal? UnitPriceBaseProductionMaximo { get; set; } = 0;
        public string Forma { get; set; } = string.Empty;
        public string Salida { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;
        public bool EliminarSolicitud { get; set; }
        public short CondicionPago { get; set; }
        public AppConversionUnitGenericCreateDto AppConversionUnitGenericCreateDto { get; set; }

    }
}
