﻿// Decompiled with JetBrains decompiler
// Type: AppService.Core.Entities.MtrCondicionPago
// Assembly: AppService.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A79E0FDF-34BB-4EE9-A48B-958643556925
// Assembly location: D:\Moore\Publish\AppService.Core.dll

using System;

namespace AppService.Core.Entities
{
    public class MtrCondicionPago
    {
        public Decimal Recnum { get; set; }

        public short Codigo { get; set; }

        public string Descripcion { get; set; }

        public short DiasVcto { get; set; }

        public string AprobCredM { get; set; }

        public string CodJde { get; set; }

        public string Inactivo { get; set; }

        public string RequiereLimiteCredito { get; set; }

        public Decimal? PorcRequerido { get; set; }

        public string CondicionSap { get; set; }

        public string OdooId { get; set; }
        
        public decimal PocGapAplicarPrecio { get; set; }
        
    }
}
