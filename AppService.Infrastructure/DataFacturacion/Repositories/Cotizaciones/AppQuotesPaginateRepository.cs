

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.DTOs.Shared;

using AppService.Core.Entities;
using AppService.Core.Interfaces.Cotizaciones;

using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace AppService.Infrastructure.Repositories.Cotizaciones
{
    public class AppQuotesPaginateRepository: IAppQuotesPaginateRepository
    {
        private readonly RRDContext _context;


        public AppQuotesPaginateRepository(RRDContext context)
        {
            _context = context;
        }

       
        public async Task<List<AppQuotesPaginate>> GetByUsuario(string usuarioConsulta)
        {
            var result= await _context.AppQuotesPaginate.Where(x=> x.UsuarioConsulta.Trim() == usuarioConsulta.Trim() ).ToListAsync();
            return result;
        }
        
        public async Task<int> GetCantidadRegistros(string usuarioConsulta)
        {
            int result = 0;
            try
            {
                var data= await _context.AppQuotesPaginate.Where(x=> x.UsuarioConsulta.Trim() == usuarioConsulta.Trim() ).FirstOrDefaultAsync();
                if (data !=  null)
                {
                    result = data.TotalRegistros;
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
           
        }
        
        public async Task UpdatePaginateQuotes(AppQuotesPaginateFilter filter)
        {
            try
            {
                string query = $"FSVEMCYN17.[RRD].[dbo].[AppUpdateQuotesPaginate] {filter.PageNumber},{filter.PageSize},'{filter.UsuarioConsulta}','{filter.SearchText}'";

                FormattableString xquery  =
                    $"insert into EjeucionProceso (IdEvaluacion, Query, Mensaje, Usuario, FechaEjecucion, IdObjetoProceso) values((SELECT MAX(IdEvaluacion)  FROM EjeucionProceso),{query},'',{filter.UsuarioConsulta},GETDATE(),31)";
                
             
                await _context.Database.ExecuteSqlInterpolatedAsync(xquery);
                
                bool continuar = false;

                while (!continuar)
                {
                    var proceso = await _context.EjeucionProceso
                        .Where(x => x.IdObjetoProceso == 31 && x.Fin != null && x.Usuario == filter.UsuarioConsulta)
                        .OrderByDescending(x => x.IdEvaluacion).FirstOrDefaultAsync();

                    if (proceso!=null)
                    {
                        continuar = true;
                    }
                }
                
                    xquery  =
                    $"delete from  EjeucionProceso where usuario ={filter.UsuarioConsulta} and IdObjetoProceso=31";
                
             
                await _context.Database.ExecuteSqlInterpolatedAsync(xquery);
                

                
                
             

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

          
        }
        
        public async Task<ResultDto<List<AppQuotesPaginate>>> GetPaginate(AppQuotesPaginateFilter filter)
        {
            ResultDto<List<AppQuotesPaginate>> result = new ResultDto<List<AppQuotesPaginate>>(null);
            
            filter.PageNumber =  filter.PageNumber+1;
            if (filter.PageSize == 0) filter.PageSize = 100;
            if (filter.PageSize >100) filter.PageSize = 100;

            if (string.IsNullOrEmpty(filter.SearchText))
            {
                filter.SearchText = "";
            }
            int totalRegistros = 0;
            int totalPage = 0;
            try
            {
                
                List<AppQuotesPaginate> pageData = new List<AppQuotesPaginate>();
                await UpdatePaginateQuotes(filter);
                totalRegistros = await GetCantidadRegistros(filter.UsuarioConsulta);
                totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                pageData = await GetByUsuario(filter.UsuarioConsulta);
                
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
