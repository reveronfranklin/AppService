﻿using AppService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Core.Interfaces
{
    public interface IMtrDireccionesRepository
    {

        Task<List<MtrDirecciones>> GetByIdCliente(string idCliente);

        Task<MtrDirecciones> GetById(decimal id);
        MtrDirecciones Update(MtrDirecciones entity);
        MtrDirecciones Add(MtrDirecciones entity);
        Task<List<MtrDirecciones>> GetByIds(List<decimal> ids);
    }
}
