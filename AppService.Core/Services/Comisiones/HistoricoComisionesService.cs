using System;
using AppService.Core.CustomEntities;
using AppService.Core.Interfaces;
using AppService.Core.Responses;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using Microsoft.IdentityModel.Tokens;

namespace AppService.Core.Services
{
    public class HistoricoComisionesService: IHistoricoComisionesService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;

        public HistoricoComisionesService(IUnitOfWork unitOfWork, IMapper mapper, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _paginationOptions = options.Value;
        }

        public async Task<ApiResponse<List<VHistoricoComsiones>>> GetByPeriodo(PeriodoFilter filter)
        {

            if (filter.SearchText == null) filter.SearchText = "";
            
            
            List<VHistoricoComsiones> resultDto = new List<VHistoricoComsiones>();

            Metadata metadata = new Metadata
            {
                IsValid = true,
                Message = ""

            };
            var update = "";
            var updateCache = await _unitOfWork.AppConfigAppRepository.GetByKey("UPDATE_CACHE");
            if (updateCache != null)
            {
                update = updateCache.Valor;
            }
            ApiResponse<List<VHistoricoComsiones>> response = new ApiResponse<List<VHistoricoComsiones>>(resultDto);

            List<VHistoricoComsiones> historico = new List<VHistoricoComsiones>();
            var vendedores =await  _unitOfWork.MtrVendedorRepository.GetAllVendedores();
            if (filter.SearchText.Length > 0  && filter.IdPeriodo==0)
            {
                historico = await _unitOfWork.VHistoricoComisionesRepository.GetBySearchText(filter,vendedores,update);
            }
            if (filter.SearchText.Length == 0 && filter.IdPeriodo>0)
            {
                historico = await _unitOfWork.VHistoricoComisionesRepository.GetByPeriodo(filter.IdPeriodo,update);
            }
            if (filter.SearchText.Length > 0 && filter.IdPeriodo>0)
            {
                historico = await _unitOfWork.VHistoricoComisionesRepository.GetByPeriodoSearchText(filter,vendedores,update);
            }
            
          
            if (historico.Count > 0)
            {
              
                var vendedor = await _unitOfWork.MtrVendedorRepository.GetByIdAsync(filter.Usuario);
                if (vendedor != null)
                {

                    response.Data = historico.Where(x => x.IdVendedor.ToUpper() == filter.Usuario.ToUpper()).ToList();
                    
                    response.Total1 = (decimal) response.Data.Sum(x => x.BsComision);
                    response.Total2 = (decimal) response.Data.Sum(x => x.BsBaseComsion);
                    response.Total3 = Math.Round((response.Total1 / response.Total2) * 100, 2);
                
                    response.Meta = metadata;
                    return response;
               

                }

                var supervisor = vendedores.Where(x => x.Supervisor.ToUpper() == filter.Usuario.ToUpper()).FirstOrDefault();
                if (supervisor != null)
                {

                    response.Data =  (from u in historico
                        join ur in vendedores on u.IdVendedor equals ur.Codigo

                        where ur.Supervisor == filter.Usuario
                        select u).ToList();

                    response.Total1 = (decimal) response.Data.Sum(x => x.BsComision);
                    response.Total2 = (decimal) response.Data.Sum(x => x.BsBaseComsion);
                    response.Total3 = Math.Round((response.Total1 / response.Total2) * 100, 2);
                
                    response.Meta = metadata;
                    return response;

                }
                
                response.Total1 = (decimal)historico.Sum(x => x.BsComision);
                response.Total2 = (decimal)historico.Sum(x => x.BsBaseComsion);
                response.Total3 = Math.Round((response.Total1 / response.Total2) * 100, 2);
                response.Data = historico;
                response.Meta = metadata;
                return response;

              
                
            }   else
            {
                response.Data = null;
                response.Meta = metadata;
                return response;
            }
            
         

           
        }

        
        public async Task<ApiResponse<List<ResumenComisionResponse>>> GetByPeriodoResumen(PeriodoFilter filter)
        {

            
            
            
            List<ResumenComisionResponse> resultDto = new List<ResumenComisionResponse>();

            Metadata metadata = new Metadata
            {
                IsValid = true,
                Message = ""

            };
            ApiResponse<List<ResumenComisionResponse>> response = new ApiResponse<List<ResumenComisionResponse>>(resultDto);


            var detalleResponse = await GetByPeriodo(filter);

            if (detalleResponse.Data == null)
            {
                return response;
            }
            
            var detalle = detalleResponse.Data;
            
   
            
            
            var resultado = detalle
                .GroupBy(p => p.Concepto)
                .Select(g => new { Concepto = g.Key,
                                                                ComisionTotal = g.Sum(p => p.BsComision),
                                                                 BaseTotal = g.Sum(p => p.BsBaseComsion) });
            List<ResumenComisionResponse> lista = new List<ResumenComisionResponse>();
            int id = 0;

            decimal baseTotal;
            baseTotal = (decimal)detalle.Sum(x => x.BsBaseComsion);
            var comisionTotal = detalle.Sum(x => x.BsComision);
            ResumenComisionResponse itemResumenTotal = new ResumenComisionResponse();
            id = id + 1;
            itemResumenTotal.Id = id;
            itemResumenTotal.Concepto = "";
            itemResumenTotal.TipoPago = "";
            itemResumenTotal.BaseComision = (decimal)baseTotal;
            itemResumenTotal.Comision = (decimal)comisionTotal;
            lista.Add(itemResumenTotal);
            
            
            foreach (var grupo in resultado)
            {
                
                ResumenComisionResponse itemResumen = new ResumenComisionResponse();
                id = id + 1;
                itemResumen.Id = id;
                itemResumen.Concepto = grupo.Concepto;
                itemResumen.TipoPago = "";
                itemResumen.BaseComision = (decimal)grupo.BaseTotal;
                itemResumen.Comision = (decimal)grupo.ComisionTotal;
                lista.Add(itemResumen);

                var detalleTipoPago = detalle.Where(x => x.Concepto == itemResumen.Concepto).ToList();
                
                var resultadoDetalle = detalleTipoPago
                    .GroupBy(p => p.DescripcionTipoPago)
                    .Select(g => new { TipoPago = g.Key,
                        ComisionTotal = g.Sum(p => p.BsComision),
                        BaseTotal = g.Sum(p => p.BsBaseComsion) });
                List<ResumenComisionResponse> listaDetalle = new List<ResumenComisionResponse> ();
                foreach (var item in resultadoDetalle)
                {
                    id = id + 1;
                    ResumenComisionResponse itemlistaDetalle = new ResumenComisionResponse ();
                    itemlistaDetalle.Id = id;
                    itemlistaDetalle.Concepto = "";
                    itemlistaDetalle.TipoPago = item.TipoPago;
                    itemlistaDetalle.Comision = (decimal)item.ComisionTotal;
                    itemlistaDetalle.BaseComision = (decimal)item.BaseTotal;
                    lista.Add(itemlistaDetalle);
                }

               
              
            }


            response.Data = lista;

            return response;
        }

        


    }
}
