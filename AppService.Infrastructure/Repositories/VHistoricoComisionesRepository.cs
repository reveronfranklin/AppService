using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataMooreve;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.Entities;
using AppService.Core.Interfaces.Comisiones;


namespace AppService.Infrastructure.Repositories
{
    public class VHistoricoComisionesRepository: IVHistoricoComisionesRepository
    {

        private readonly MooreveContext _context;
        private readonly IAppCache _cache;

        public VHistoricoComisionesRepository(MooreveContext context,IAppCache cache)
        {
            _context = context;
            _cache = cache;
        }

        
        public async Task SetCacheAsync(string key, string value)
        {
            await _cache.SetAsync(key, value,TimeSpan.FromDays(30));
        }
        public void RemoveCache(string key)
        {
            _cache.Remove(key);
        }
        public async Task<string> GetCacheAsync(string key)
        {
            return await _cache.GetAsync(key);
        }

        
        
            
        public async Task<List<VHistoricoComsiones>> GetByPeriodoSearchText(PeriodoFilter filter,List<MtrVendedor> vendedores,string update)
        {


            var historico = await _context.VHistoricoComsiones.Where(x=> 
                    x.IdPeriodo==filter.IdPeriodo && x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                .OrderBy(x=>x.Orden)
                .ThenBy(x=>x.Concepto)
                .ThenBy(x=>x.DescripcionTipoPago)
                .ToListAsync();
            
            var vendedor = vendedores.Where(x => x.Codigo == filter.Usuario).FirstOrDefault();
            if (vendedor != null)
            {
                historico = historico.Where(x=> 
                        x.IdVendedor.ToUpper() == filter.Usuario.ToUpper() )
                    .OrderBy(x=>x.Orden)
                    .ThenBy(x=>x.Concepto)
                    .ThenBy(x=>x.DescripcionTipoPago)
                    .ToList();
               
         

            }

            var supervisor = vendedores.Where(x => x.Supervisor.ToUpper() == filter.Usuario.ToUpper()).FirstOrDefault();
            if (supervisor != null)
            {

                historico =  (from u in historico
                    join ur in vendedores on u.IdVendedor equals ur.Codigo

                    where ur.Supervisor == filter.Usuario
                    select u).ToList();

           
            }

         
            return historico;
          
        }
        
        public async Task<List<VHistoricoComsiones>> GetBySearchText(PeriodoFilter filter,List<MtrVendedor> vendedores,string update)
        {

            List<VHistoricoComsiones> historico = new List<VHistoricoComsiones>();
            if(update=="X")  RemoveCache($"ListComisionesPeriodo{filter.Usuario}{filter.SearchText}");
            var listHistorico=await GetCacheAsync($"ListComisionesPeriodo{filter.Usuario}{filter.SearchText}");
            if (listHistorico != null)
            {
                historico = System.Text.Json.JsonSerializer.Deserialize<List<VHistoricoComsiones>> (listHistorico);
                return historico;
            }

        
            
            historico = await _context.VHistoricoComsiones.Where(x=> 
                    x.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                .OrderBy(x=>x.Orden)
                .ThenBy(x=>x.Concepto)
                .ThenBy(x=>x.DescripcionTipoPago)
                .ToListAsync();
            
            var vendedor = vendedores.Where(x => x.Codigo == filter.Usuario).FirstOrDefault();
            if (vendedor != null)
            {
                historico = historico.Where(x=> 
                        x.IdVendedor.ToUpper() == filter.Usuario.ToUpper() )
                    .OrderBy(x=>x.Orden)
                    .ThenBy(x=>x.Concepto)
                    .ThenBy(x=>x.DescripcionTipoPago)
                    .ToList();
               
         

            }

            var supervisor = vendedores.Where(x => x.Supervisor.ToUpper() == filter.Usuario.ToUpper()).FirstOrDefault();
            if (supervisor != null)
            {

                historico =  (from u in historico
                    join ur in vendedores on u.IdVendedor equals ur.Codigo

                    where ur.Supervisor == filter.Usuario
                    select u).ToList();

           
            }

            
            
         
            if (historico.Count > 0)
            {
                await SetCacheAsync($"ListComisionesPeriodo{filter.Usuario}{filter.SearchText}", System.Text.Json.JsonSerializer.Serialize(historico));
            }
        
          
            return historico;
          
        }
        
        public async Task<List<VHistoricoComsiones>> GetByPeriodo(int idPeriodo, string update)
        {

            
           
            List<VHistoricoComsiones> result = new List<VHistoricoComsiones>();
            if(update=="X" ) RemoveCache($"ListComisionesPeriodo{idPeriodo}");
            var listHistorico=await GetCacheAsync($"ListComisionesPeriodo{idPeriodo}");
            if (listHistorico != null)
            {
                result = System.Text.Json.JsonSerializer.Deserialize<List<VHistoricoComsiones>> (listHistorico);
                return result;
            }
            
          result = await _context.VHistoricoComsiones.Where(x=>x.IdPeriodo==idPeriodo)
              .OrderBy(x=>x.Orden)
              .ThenBy(x=>x.Concepto)
              .ThenBy(x=>x.DescripcionTipoPago)
              .ToListAsync();
          if (result.Count > 0)
          {
              await SetCacheAsync($"ListComisionesPeriodo{idPeriodo}", System.Text.Json.JsonSerializer.Serialize(result));
          }
        
          
          return result;
          
        }
    
     

    


    }
}
