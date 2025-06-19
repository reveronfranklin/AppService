
using System;
using AppService.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.DTOs.PowerBi;
using AppService.Core.DTOs.Shared;
using AppService.Core.Entities;
using AppService.Core.EntitiesPowerBI;
using AppService.Core.Interfaces.Cotizaciones;
using AppService.Core.Interfaces.PowerBi;


namespace AppService.Core.Services.PowerBi
{
    public class PowerBiOrdenesService:IPowerBiOrdenesService
    {

        private readonly IUnitOfWork _unitOfWork;
     

        public PowerBiOrdenesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
          
        }

        public async Task<List<Ordene>> GetAll(PowerBiOrdenesFilter filter)
        {
            bool esVendedor = false;
            var vendedor = await _unitOfWork.MtrVendedorRepository.GetByIdAsync(filter.Usuario);
            if (vendedor == null)
            {
                esVendedor = false;
            }
            else
            {
                if (vendedor.Codigo != vendedor.Supervisor)
                {
                    esVendedor = true;
                }
                if (vendedor.Supervisor=="")
                {
                    esVendedor = false;
                }
                
            }
            return await _unitOfWork.PowerBiOrdenesRepository.GetAll(filter.Usuario,esVendedor,filter.Year);
        }

  
        public async Task<List<VVenta>> GetAllVentas()
        {
            decimal year = DateTime.Now.Year -1;
            return await _unitOfWork.PowerBiOrdenesRepository.GetAllVentas(year);
        }
   
        public async Task<List<AppService.Core.EntitiesPowerBI.Ventas>> GetAllVentasReducido()
        {
            decimal year = DateTime.Now.Year;
            return await _unitOfWork.PowerBiOrdenesRepository.GetAllVentasReducido(year);
        }


    }
}
