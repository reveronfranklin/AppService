﻿// Decompiled with JetBrains decompiler
// Type: AppService.Core.Entities.GssGrupoTrabajo
// Assembly: AppService.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A79E0FDF-34BB-4EE9-A48B-958643556925
// Assembly location: D:\Moore\Publish\AppService.Core.dll

using System;

namespace AppService.Core.Entities
{
  public class GssGrupoTrabajo
  {
    public int IdGrupoTabajo { get; set; }

    public string NombreGrupoTrabajo { get; set; }

    public int? IdUnidadServicio { get; set; }

    public DateTime FechaCambio { get; set; }

    public int UsuarioCambio { get; set; }
  }
}