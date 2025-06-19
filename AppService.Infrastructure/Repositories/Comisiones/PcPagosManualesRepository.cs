
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces.Comisiones;
using AppService.Infrastructure.DataMooreve;

namespace AppService.Infrastructure.Repositories.Comisiones
{
    public class PcPagosManualesRepository: IPcPagosManualesRepository
    {
        private readonly MooreveContext _context;


        public PcPagosManualesRepository(MooreveContext context)
        {
            _context = context;
        }

        public async Task<List<Wsmy685>> GetAll()
        {

            return await EntityFrameworkQueryableExtensions.ToListAsync(_context.Wsmy685);

        }

        public async Task<ResultDto<List<Wsmy685>>> GetPaginate(PagosManualesFilter filter)
        {
            ResultDto<List<Wsmy685>> result = new ResultDto<List<Wsmy685>>(null);
            
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
                
                var updateSearchText = await UpdateSearchText();
                
                List<Wsmy685> pageData = new List<Wsmy685>();

            
                if (filter.SearchText.Length>0)
                {
                    totalRegistros = _context.Wsmy685
                        .Where(x => x.FlagPagado==false && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .Count();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData = _context.Wsmy685
                        .Where(x => x.FlagPagado==false && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .ToList();
                    
                }
                else
                {
                    totalRegistros = _context.Wsmy685.Where(x=>x.FlagPagado==false)
                        .Count();

                    totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                    pageData = _context.Wsmy685
                        .Where(x=>x.FlagPagado==false)
                        //.Skip((filter.PageNumber - 1) * filter.PageSize)
                        //.Take(filter.PageSize)
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
        

        public async Task<Wsmy685> GetById(long id)
        {
            return await _context.Wsmy685.FindAsync(id);
        }
        
        public async Task Add(Wsmy685 entity)
        {
            await _context.Wsmy685.AddAsync(entity);


        }

        public void Update(Wsmy685 entity)
        {
            _context.Wsmy685.Update(entity);

        }

        public async Task Delete(long id)
        {
            Wsmy685 entity = await GetById(id);
            _context.Wsmy685.Remove(entity);

        }
        
        public async Task<string> UpdateSearchText()
        {

            try
            {
                FormattableString xqueryDiario = $"UPDATE MOOREVE.DBO.WSMY685 SET MOOREVE.DBO.WSMY685.SearchText = TRANSACCION  + cast(ORDEN as nvarchar(20)) + PRODUCTO + OBSERVACIONES ";

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
