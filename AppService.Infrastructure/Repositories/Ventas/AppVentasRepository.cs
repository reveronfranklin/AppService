

using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Shared;
using AppService.Core.DTOs.Ventas;
using AppService.Core.Entities;
using AppService.Core.Interfaces.Ventas;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace AppService.Infrastructure.Repositories.Ventas
{
    public class AppVentasRepository: IAppVentasRepository
    {
        private readonly RRDContext _context;


        public AppVentasRepository(RRDContext context)
        {
            _context = context;
        }

       
        public async Task<AppVentas> GetByOrdenProducto(decimal orden,string producto)
        {
            var result= await _context.AppVentas.Where(x=> x.Orden == orden && x.Producto==producto).FirstOrDefaultAsync();
            return result;
        }
        
        public async Task<AppVentas> GetByOrden(decimal orden)
        {
            var result= await _context.AppVentas.Where(x=> x.Orden == orden ).FirstOrDefaultAsync();
            return result;
        }
        
        public async Task<ResultDto<List<AppVentas>>> GetPaginate(AppVentasFilter filter)
        {
            ResultDto<List<AppVentas>> result = new ResultDto<List<AppVentas>>(null);
            
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
                
               
                
                List<AppVentas> pageData = new List<AppVentas>();

            
                if (filter.SearchText.Length>0)
                {
                    totalRegistros = _context.AppVentas
                        .Where(x => x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .Count();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData = _context.AppVentas
                        .Where(x => x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .Skip((filter.PageNumber - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .OrderByDescending(x=>x.Orden)
                        .ToList();
                    
                }
                else
                {
                    totalRegistros = _context.AppVentas
                        .Count();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData = _context.AppVentas
                     
                        .Skip((filter.PageNumber - 1) * filter.PageSize)
                        .Take(filter.PageSize)
                        .OrderByDescending(x=>x.Orden)
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
        

    }
}
