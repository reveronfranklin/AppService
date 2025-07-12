using AppService.Core.EntitiesClientes;
using AppService.Core.Interfaces;
using AppService.Infrastructure.DataClientes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
    public class Wsmy264Repository:IWsmy264Repository
    {

        private readonly ClientesContext _context;

        public Wsmy264Repository(ClientesContext context)
        {
            _context = context;
        }

        public async Task<Wsmy264> GetById(decimal id)
        {
            return await _context.Wsmy264.Where(x=>x.Id==id).FirstOrDefaultAsync();
        }
        public async Task<Wsmy264> GetByCodigoIdDireccion(decimal id,decimal codigo)
        {
            return await _context.Wsmy264.Where(x=>x.IdDireccion==id && x.Codigo==codigo).FirstOrDefaultAsync();
        }
        public async Task Add(Wsmy264 entity)
        {
            await _context.Wsmy264.AddAsync(entity);


        }



        public void Update(Wsmy264 entity)
        {
            _context.Wsmy264.Update(entity);

        }
        public void UpdateQuery(Wsmy264 entity)
        {
            var query = "UPDATE WSMY264 SET Rif ={1},Direccion={2}, Direccion_1={3}, Direccion_2={4},ESTADO={5},MUNICIPIO={6},PuntoReferencia={7},ORIGEN={8} WHERE Id={0}";
            try
            {
                var result = _context.Database.ExecuteSqlRaw(query,entity.Id,entity.Rif,entity.Direccion,entity.Direccion1,entity.Direccion2,entity.Estado,entity.Municipio,entity.PuntoReferencia,entity.Origen);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
         
        }
        public void AddQuery(Wsmy264 entity)
        {
            //var query = "UPDATE WSMY264 SET Rif ={1},Direccion={2}, Direccion_1={3}, Direccion_2={4},ESTADO={5},MUNICIPIO={6} WHERE Id={0}";
            var query =
                "INSERT INTO WSMY264 (Codigo, Rif, Id_Direccion,Direccion, Direccion_1, Direccion_2, ESTADO, MUNICIPIO,PuntoReferencia,nuevo,origen) VALUES ({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10})";

            try
            {
                var result = _context.Database.ExecuteSqlRaw(query,entity.Codigo,entity.Rif,entity.IdDireccion,entity.Direccion,entity.Direccion1,entity.Direccion2,entity.Estado,entity.Municipio,entity.PuntoReferencia,"",entity.Origen);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
         
        }

        public async Task Delete(decimal id)
        {
            Wsmy264 entity = await GetById(id);
            _context.Wsmy264.Remove(entity);

        }

        public async Task<decimal> NextId(decimal codigo)
        {
            decimal result;
           
            var ultimo = await _context.Wsmy264.Where(x=>x.Codigo==codigo).OrderByDescending(x=>x.IdDireccion).FirstOrDefaultAsync();
            if (ultimo != null)
            {
                result = ultimo.IdDireccion + 1;
            }
            else
            {
                result = 1;
            }
            
            return result;

        }
     

    }
}
