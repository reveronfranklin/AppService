﻿// Decompiled with JetBrains decompiler
// Type: AppService.Core.Entities.AppDetailQuotes
// Assembly: AppService.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A79E0FDF-34BB-4EE9-A48B-958643556925
// Assembly location: D:\Moore\Publish\AppService.Core.dll

using System;
using System.Collections.Generic;

namespace AppService.Core.Entities
{
    public class AppDetailQuotes
    {
        public AppDetailQuotes() => this.AppDetailQuotesConversionUnit = (ICollection<AppService.Core.Entities.AppDetailQuotesConversionUnit>)new HashSet<AppService.Core.Entities.AppDetailQuotesConversionUnit>();

        public int Id { get; set; }

        public int AppGeneralQuotesId { get; set; }

        public string Cotizacion { get; set; }

        public string Producto { get; set; }

        public int IdProducto { get; set; }

        public string NombreComercialProducto { get; set; }

        public int IdEstatus { get; set; }

        public Decimal Cantidad { get; set; }

        public Decimal Precio { get; set; }

        public Decimal Total { get; set; }

        public Decimal PrecioUsd { get; set; }

        public Decimal TotalUsd { get; set; }

        public int IdUnidad { get; set; }

        public string Observaciones { get; set; }

        public int DiasEntrega { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string UserCreate { get; set; }

        public string UserUpdate { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Decimal? ValorConvertido { get; set; }

        public Decimal? UnitPriceBaseProduction { get; set; }

        public Decimal? UnitPriceConverted { get; set; }

        public int? RazonGanadaPerdida { get; set; }

        public int? Competidor { get; set; }

        public int? QuantityPerPackage { get; set; }

        public string ObsSolicitud { get; set; }

        public Decimal? CantidadSolicitada { get; set; }

        public Decimal? CantidadPorUnidadProduccion { get; set; }

        public bool? SolicitarPrecio { get; set; }

        public Decimal? Rprecio { get; set; }

        public Decimal? Rtotal { get; set; }

        public int? CalculoId { get; set; }

        public Decimal? MedidaBasica { get; set; }

        public Decimal? MedidaOpuesta { get; set; }

        public int? OdooId { get; set; }

        public long? OrdenAnterior { get; set; }

        public string Papeles { get; set; } = string.Empty;
        public string TipoPapeles { get; set; } = string.Empty;
        public string Tintas { get; set; } = string.Empty;
        public string Medidas { get; set; } = string.Empty;
        public int CantPartes { get; set; } = 0;
        public int CantTintas { get; set; } = 0;


        public Decimal? UnitPriceBaseProductionMaximo { get; set; } = 0;


        public string Forma { get; set; } = string.Empty;
        public string Salida { get; set; } = string.Empty;
        public string Presentacion { get; set; } = string.Empty;

        public virtual AppGeneralQuotes AppGeneralQuotes { get; set; }

        public virtual AppStatusQuote IdEstatusNavigation { get; set; }

        public virtual AppProducts IdProductoNavigation { get; set; }

        public virtual AppUnits IdUnidadNavigation { get; set; }

        public string MedidaBasicaFraccionPulgada { get; set; }

        public string? MedidaOpuestaFraccionPulgada { get; set; }
        
        public short IdCondPago { get; set; }
        public decimal? PorcFlete { get; set; }
        public decimal? Flete { get; set; }

        public virtual ICollection<AppService.Core.Entities.AppDetailQuotesConversionUnit> AppDetailQuotesConversionUnit { get; set; }
    }
}
