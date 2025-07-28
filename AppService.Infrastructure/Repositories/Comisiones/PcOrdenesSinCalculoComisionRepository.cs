
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using AppService.Infrastructure.DataMooreve;

namespace AppService.Infrastructure.Repositories.Comisiones
{
    public class PcOrdenesSinCalculoComisionRepository:IPcOrdenesSinCalculoComisionRepository
    {
        private readonly MooreveContext _context;


        public PcOrdenesSinCalculoComisionRepository(MooreveContext context)
        {
            _context = context;
        }

      
        public async Task<Core.DTOs.Shared.ResultDto<List<PcOrdenesSinCalculoComision>>> GetPaginate(PagosManualesFilter filter)
        {
            Core.DTOs.Shared.ResultDto<List<PcOrdenesSinCalculoComision>> result = new Core.DTOs.Shared.ResultDto<List<PcOrdenesSinCalculoComision>>(null);
            
            filter.PageNumber =  filter.PageNumber+1;
            if (filter.PageSize == 0) filter.PageSize = 100;
            if (filter.PageSize >100) filter.PageSize = 100;

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
                    totalRegistros = _context.PcOrdenesSinCalculoComision
                        .Where(x => x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .Count();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData = _context.PcOrdenesSinCalculoComision
                        .Where(x =>  x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .Skip((filter.PageNumber - 1) * filter.PageSize)
                        .Take(filter.PageSize)

                        .ToList();
                    
                }
                else
                {
                    totalRegistros = _context.PcOrdenesSinCalculoComision
                        .Count();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData = _context.PcOrdenesSinCalculoComision
                       
                        .Skip((filter.PageNumber - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .ToList();

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
                FormattableString xqueryDiario = $"UPDATE MOOREVE.DBO.PcOrdenesSinCalculoComision SET MOOREVE.DBO.WSMY685.SearchText = cast(ORDEN as nvarchar(20)) WHERE (SearchText IS NULL)";

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
