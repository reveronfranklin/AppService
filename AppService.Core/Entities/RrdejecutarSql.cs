﻿// Decompiled with JetBrains decompiler
// Type: AppService.Core.Entities.RrdejecutarSql
// Assembly: AppService.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A79E0FDF-34BB-4EE9-A48B-958643556925
// Assembly location: D:\Moore\Publish\AppService.Core.dll

using System;

namespace AppService.Core.Entities
{
  public class RrdejecutarSql
  {
    public Decimal Id { get; set; }

    public string Proceso { get; set; }

    public string QuerySql { get; set; }

    public string Ejecutar { get; set; }
  }
}
