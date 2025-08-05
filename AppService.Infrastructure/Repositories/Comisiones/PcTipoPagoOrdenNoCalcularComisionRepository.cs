
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.Entities;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using AppService.Infrastructure.Data;


namespace AppService.Infrastructure.Repositories.Comisiones
{
    public class PcTipoPagoOrdenNoCalcularComisionRepository:IPcTipoPagoOrdenNoCalcularComisionRepository
    {
        private readonly RRDContext _context;


        public PcTipoPagoOrdenNoCalcularComisionRepository(RRDContext context)
        {
            _context = context;
        }

      
        public async Task<Core.DTOs.Shared.ResultDto<List<PCTipoPagoOrdenNoCalcularComision>>> GetPaginate(PagosManualesFilter filter)
        {
            Core.DTOs.Shared.ResultDto<List<PCTipoPagoOrdenNoCalcularComision>> result = new Core.DTOs.Shared.ResultDto<List<PCTipoPagoOrdenNoCalcularComision>>(null);
            
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
                
                List<PCTipoPagoOrdenNoCalcularComision> pageData = new List<PCTipoPagoOrdenNoCalcularComision>();

            
                if (filter.SearchText.Length>0)
                {
                    totalRegistros =await _context.PCTipoPagoOrdenNoCalcularComision
                        .Where(x =>x.Activo==true && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .CountAsync();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData =await  _context.PCTipoPagoOrdenNoCalcularComision
                        .Where(x => x.Activo==true && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .OrderBy(x => x.Id) 

                        .Skip((filter.PageNumber - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .ToListAsync();
                    
                }
                else
                {
                    totalRegistros =await _context.PCTipoPagoOrdenNoCalcularComision
                        .Where(x=> x.Activo == true)
                        .CountAsync();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData = await  _context.PCTipoPagoOrdenNoCalcularComision
                        .Where(x=> x.Activo == true)
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
        

        public async Task<PCTipoPagoOrdenNoCalcularComision> GetById(long id)
        {
            
            return await _context.PCTipoPagoOrdenNoCalcularComision.Where(x=>x.Id==id && x.Activo==true).FirstOrDefaultAsync();
        }
        
        public async Task<PCTipoPagoOrdenNoCalcularComision> GetByOrden(long orden)
        {
            
            return await _context.PCTipoPagoOrdenNoCalcularComision.Where(x=>x.Activo==true && x.Orden==orden).FirstOrDefaultAsync();
        }
        
        public async Task<PCTipoPagoOrdenNoCalcularComision> GetByTipoPagoOrden(int tipoPago,long orden)
        {
            
            return await _context.PCTipoPagoOrdenNoCalcularComision.Where(x=>x.Activo==true && x.IdTipoPago==tipoPago && x.Orden==orden).FirstOrDefaultAsync();
        }
        
        public async Task Add(PCTipoPagoOrdenNoCalcularComision entity)
        {
            entity.SearchText = entity.Orden.ToString() + entity.Cliente;
            entity.Activo = true;
            
            await _context.PCTipoPagoOrdenNoCalcularComision.AddAsync(entity);


        }

        public void Update(PCTipoPagoOrdenNoCalcularComision entity)
        {
            entity.SearchText = entity.Orden.ToString() + entity.Cliente;
            _context.PCTipoPagoOrdenNoCalcularComision.Update(entity);

        }

        public async Task Delete(long id)
        {
            PCTipoPagoOrdenNoCalcularComision entity = await GetById(id);
            entity.SearchText = entity.Orden.ToString() + entity.Cliente;
            entity.Activo = false;
            _context.PCTipoPagoOrdenNoCalcularComision.Update(entity);

        }
        
        public async Task<string> UpdateSearchText()
        {

            try
            {
                FormattableString xqueryDiario = $"UPDATE RRD.DBO.PCTipoPagoOrdenNoCalcularComision SET RRD.DBO.PCTipoPagoOrdenNoCalcularComision.SearchText = cast(ORDEN as nvarchar(20)) + Cliente WHERE (SearchText IS NULL)";

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
