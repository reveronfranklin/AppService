
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using AppService.Infrastructure.Data;


namespace AppService.Infrastructure.Repositories.Comisiones
{
    public class PcOrdenesSinCalculoComisionRepository:IPcOrdenesSinCalculoComisionRepository
    {
        private readonly RRDContext _context;


        public PcOrdenesSinCalculoComisionRepository(RRDContext context)
        {
            _context = context;
        }

      /*
       *
       *  if (filter.PageNumber == 0) filter.PageNumber = 1;
                   if (filter.PageSize == 0) filter.PageSize = 100;
                   if (filter.PageSize >100) filter.PageSize = 100;
                   if (filter.SearchText == null) filter.SearchText = "";
                   var totalRegistros = 0;
                   var totalPage = 0;
                   List<PRE_V_SALDOS> preVSaldos;

                   if ( filter.SearchText.Length > 0)
                   {
                       preVSaldos = await _context.PRE_V_SALDOS.
                           Where(x => 
                               x.CODIGO_PRESUPUESTO == filter.CodigoPresupuesto && 
                               x.DISPONIBLE>0 &&
                               x.SEARCH_TEXT.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                           .OrderBy(x=>x.CODIGO_ICP_CONCAT)
                           .ThenBy(x=> x.CODIGO_PUC_CONCAT)
                           .Skip((filter.PageNumber - 1) * filter.PageSize)
                           .Take(filter.PageSize)
                           .ToListAsync();
       * 
       */
        public async Task<Core.DTOs.Shared.ResultDto<List<PcOrdenesSinCalculoComision>>> GetPaginate(PagosManualesFilter filter)
        {
            Core.DTOs.Shared.ResultDto<List<PcOrdenesSinCalculoComision>> result = new Core.DTOs.Shared.ResultDto<List<PcOrdenesSinCalculoComision>>(null);
            
            if (filter.PageNumber == 0) filter.PageNumber = 1;
            if (filter.PageSize == 0) filter.PageSize = 100;
            if (filter.PageSize >100) filter.PageSize = 100;
            if (filter.SearchText == null) filter.SearchText = "";

            if (string.IsNullOrEmpty(filter.SearchText))
            {
                filter.SearchText = "";
            }
            var totalRegistros = 0;
            var totalPage = 0;
            try
            {

                await UpdateSearchText();
                
                List<PcOrdenesSinCalculoComision> pageData = new List<PcOrdenesSinCalculoComision>();

            
                if (filter.SearchText.Length>0)
                {
                    totalRegistros =await _context.PcOrdenesSinCalculoComision
                        .Where(x => x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .CountAsync();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData =await  _context.PcOrdenesSinCalculoComision
                        .Where(x =>  x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .OrderBy(x => x.Id) 

                        .Skip((filter.PageNumber - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .ToListAsync();
                    
                }
                else
                {
                    totalRegistros =await _context.PcOrdenesSinCalculoComision
                        .CountAsync();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData = await  _context.PcOrdenesSinCalculoComision
                        .OrderBy(x => x.Id) // Debes especificar un orden para paginación consistente
                        .Skip((filter.PageNumber - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .ToListAsync();
                    
                    
               
                    
                    

                }
                
             

                if (filter.PageNumber > 0) filter.PageNumber = filter.PageNumber - 1;
                result.CantidadRegistros = totalRegistros;
                result.TotalPage = totalPage;
                result.Page = filter.PageNumber;
                result.IsValid = true;
                result.Message = "";
                result.Data = pageData;
                return result;
                
            }
            catch (Exception e)
            {
                result.CantidadRegistros = 0;
                result.IsValid = false;
                result.Message = e.Message;
                result.Data = null;
                return result;
            }
            
        } 
        

        public async Task<PcOrdenesSinCalculoComision> GetById(long id)
        {
            
            return await _context.PcOrdenesSinCalculoComision.Where(x=>x.Id==id).FirstOrDefaultAsync();
        }
        
        public async Task<PcOrdenesSinCalculoComision> GetByOrden(long orden)
        {
            
            return await _context.PcOrdenesSinCalculoComision.Where(x=>x.Orden==orden).FirstOrDefaultAsync();
        }
        
        public async Task Add(PcOrdenesSinCalculoComision entity)
        {
            entity.SearchText = entity.Orden.ToString();
            
            await _context.PcOrdenesSinCalculoComision.AddAsync(entity);


        }

        public void Update(PcOrdenesSinCalculoComision entity)
        {
            entity.SearchText = entity.Orden.ToString();
            _context.PcOrdenesSinCalculoComision.Update(entity);

        }

        public async Task Delete(long id)
        {
            PcOrdenesSinCalculoComision entity = await GetById(id);
            _context.PcOrdenesSinCalculoComision.Remove(entity);

        }
        
        public async Task<string> UpdateSearchText()
        {

            try
            {
                FormattableString xqueryDiario = $"UPDATE RRD.DBO.PcOrdenesSinCalculoComision SET MOOREVE.DBO.PcOrdenesSinCalculoComision.SearchText = cast(ORDEN as nvarchar(20)) + Cliente WHERE (SearchText IS NULL)";

                var resultDiario = _context.Database.ExecuteSqlInterpolated(xqueryDiario);
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }




        }

    }
}
