using AppService.Core.DTOs;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Infrastructure.Repositories
{
 // DTO para la respuesta paginada
   

    public class MtrClienteRepository : IMtrClienteRepository
    {
        private readonly RRDContext _context;


        public MtrClienteRepository(RRDContext context)
        {
            _context = context;

        }

        public IEnumerable<MtrCliente> GetAll(string usuario)
        {

            return _context.MtrCliente.AsEnumerable();

        }

        public async Task<List<MtrCliente>> GetAllActive(string region)
        {

            return await _context.MtrCliente.Where(x => x.CodigoRegion == region).ToListAsync();

        }

        public async Task<List<MtrCliente>> GetAllDAyUpdate(int days)
        {
            
            DateTime localDate = DateTime.Now.AddDays(-days);
            string fechaString = localDate.ToString("dd-MM-yyyy");
            DateTime dateTime = DateTime.Parse(fechaString);
            var result =  await _context.MtrCliente.Where(x => x.FModificacion >= dateTime).ToListAsync();
            return result;

        }


        public async Task<List<MtrCliente>> GetAllByRegion(string region)
        {

            return await _context.MtrCliente.ToListAsync();

        }

        public async Task<MtrCliente> GetByIdAsync(string id)
        {

            return await _context.MtrCliente.Where(x => x.Codigo.Trim() == id.Trim()).FirstOrDefaultAsync();

        }

        public async Task<MtrCliente> GetByRifAndDifferentVendedorAsync(string rif, string vendedor)
        {
            var rifNormalizado = (rif ?? string.Empty).Trim().ToUpper();
            var vendedorNormalizado = (vendedor ?? string.Empty).Trim().ToUpper();

            return await _context.MtrCliente
                .Where(x =>
                    x.NoRegTribut != null &&
                    x.NoRegTribut.Trim().ToUpper() == rifNormalizado &&
                    x.Codigo.Trim() != "000000" &&
                    (x.Vendedor1 ?? string.Empty).Trim().ToUpper() != vendedorNormalizado)
                .FirstOrDefaultAsync();
        }

        public MtrCliente GetById(string id)
        {
            try
            {
                var cliente = _context.MtrCliente.Where(x => x.Codigo.Trim() == id.Trim()).FirstOrDefault();
                return cliente;
            }
            catch (System.Exception e)
            {
                var msg = e.InnerException.Message;
                return null;
            }


        }

        public MtrCliente Update(MtrCliente mtrCliente)
        {
            try
            {
                _context.MtrCliente.Update(mtrCliente);
                return mtrCliente;
            }
            catch (System.Exception e)
            {
                var msg = e.InnerException.Message;
                return null;
            }


        }


        public async Task<IEnumerable<MtrCliente>> ListClientesPorUsuarioBk(MtrClienteQueryFilter filter)
        {
            List<MtrCliente> result = new List<MtrCliente>();
            string usuario = filter.Usuario;
            int pageSize = filter.PageSize;


            var supervisor = await _context.MtrVendedor.Where(x => x.Supervisor == usuario).FirstOrDefaultAsync();
            if (supervisor != null)
            {

                result = await (from u in _context.MtrCliente
                                join ur in _context.MtrVendedor on u.Vendedor1 equals ur.Codigo

                                where ur.Supervisor == usuario & u.FlagInactivo != "X"
                                select u).ToListAsync();





                var clieProspecto = await _context.MtrCliente.Where(x => x.Codigo == "000000").ToListAsync();
                result.AddRange(clieProspecto);
                return result;

            }


            var vendedor = await _context.MtrVendedor.Where(x => x.Codigo == usuario).FirstOrDefaultAsync();
            if (vendedor != null)
            {


                if (filter.Codigo != null && filter.Codigo.Length > 0)
                {
                    result = await _context.MtrCliente.Where(x => x.Vendedor1 == usuario && x.FlagInactivo != "X" && ((x.Codigo.Trim().ToLower().Contains(filter.Codigo.Trim().ToLower())) || (x.NoRegTribut.Trim().ToLower().Contains(filter.Codigo.Trim().ToLower())) || (x.Nombre.Trim().ToLower().Contains(filter.Codigo.Trim().ToLower())))).OrderBy(x => x.Nombre).Skip((filter.PageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                }
                else if (filter.SearchText != null && filter.SearchText.Length > 0)
                {
                    result = await _context.MtrCliente.Where(x => x.Vendedor1 == usuario && x.FlagInactivo != "X" && ((x.Codigo.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())) || (x.NoRegTribut.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())) || (x.Nombre.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())))).OrderBy(x => x.Nombre).Skip((filter.PageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                }else
                {
                    result = await _context.MtrCliente.Where(x => x.Vendedor1 == usuario && x.FlagInactivo != "X").OrderBy(x => x.Nombre).Skip((filter.PageNumber - 1) * pageSize).Take(pageSize).ToListAsync();


                }






                var clieProspecto = await _context.MtrCliente.Where(x => x.Codigo == "000000").ToListAsync();
                result.AddRange(clieProspecto);
                return result;


            }

           


            var usuarioOficina = await _context.MtrUsuarioOficina.Where(x => x.Usuario == usuario).ToListAsync();
            if (usuarioOficina.Count > 0)
            {
                if (filter.Codigo != null && filter.Codigo.Length > 0)
                {

                    var clie = await _context.MtrCliente.Where(x => x.FlagInactivo != "X" && ((x.Codigo.Trim().ToLower().Contains(filter.Codigo.Trim().ToLower())) || (x.NoRegTribut.Trim().ToLower().Contains(filter.Codigo.Trim().ToLower())) || (x.Nombre.Trim().ToLower().Contains(filter.Codigo.Trim().ToLower())))).OrderBy(x => x.Nombre).Skip((filter.PageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

                    result.AddRange(clie);
                    //foreach (var itemcli in clie)
                    //{
                    //    result.Add(itemcli);
                    //}



                }
                else
                {
                    long number1 = 0;
                    bool canConvert = long.TryParse(filter.SearchText.Trim(), out number1);
                    if (canConvert == true)
                    {
                        var clie = await _context.MtrCliente.Where(x => x.FlagInactivo != "X" && ((x.Codigo.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))  )).OrderBy(x => x.Nombre).Skip((filter.PageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                        result.AddRange(clie);
                    }
                    else
                    {
                        var clie = await _context.MtrCliente.Where(x => x.FlagInactivo != "X" && ((x.Codigo.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())) || (x.NoRegTribut.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())) || (x.Nombre.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())))).OrderBy(x => x.Nombre).Skip((filter.PageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
                        result.AddRange(clie);
                    }
                    //var clie = await _context.MtrCliente.Where(x => x.FlagInactivo != "X").OrderBy(x => x.Nombre).ToListAsync();
                    
                   



                }



            }

            if (filter.Oficina != null)
            {
                result = result.Where(x => x.OficinaVenta.Trim() == filter.Oficina.Trim()).ToList();
            }
            if (filter.Vendedor != null)
            {
                result = result.Where(x => x.Vendedor1.ToLower().Trim() == filter.Vendedor.ToLower().Trim()).ToList();
            }

            var clieProspectoNew = await _context.MtrCliente.Where(x => x.Codigo == "000000").ToListAsync();
            result.AddRange(clieProspectoNew);

            return result;

        }


        public async Task<List<MtrCliente>> ListCliente(MtrClienteQueryFilter filter)
        {
            List<MtrCliente> result = new List<MtrCliente>();
            string usuario = filter.Usuario;
            int pageSize = filter.PageSize;
            var clie = await _context.MtrCliente.Where(x => x.Codigo == filter.Codigo).ToListAsync();

            result.AddRange(clie);



            return result;

        }



        public async Task AddMtrClienteEnvioOdooLog(MtrClienteEnvioOdooLog entity)
        {

            try
            {
                await _context.MtrClienteEnvioOdooLog.AddAsync(entity);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
    


        }



        //Refactore Busqueda Cliente Por Usuario


        private async Task<List<MtrCliente>> AddClienteProspecto(List<MtrCliente> clientes)
        {
            var tieneProspecto = clientes.Any(x => x.Codigo == "000000");
            
            if (!tieneProspecto)
            {
                var clienteProspecto = await _context.MtrCliente
                    .Where(x => x.Codigo == "000000")
                    .FirstOrDefaultAsync();
                
                if (clienteProspecto != null)
                {
                    clientes.Insert(0, clienteProspecto);
                }
            }
            
            return clientes;
        }
        private IQueryable<MtrCliente> ApplyAdditionalFilters(IQueryable<MtrCliente> query, MtrClienteQueryFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Oficina))
            {
                query = query.Where(x => x.OficinaVenta.Trim() == filter.Oficina.Trim());
            }
            
            if (!string.IsNullOrWhiteSpace(filter.Vendedor))
            {
                query = query.Where(x => x.Vendedor1.ToLower().Trim() == filter.Vendedor.ToLower().Trim());
            }
            
            return query;
        }

        private IQueryable<MtrCliente> GetClientesPorOficina(MtrClienteQueryFilter filter)
        {
            var query = _context.MtrCliente.Where(x => x.FlagInactivo != "X");
            var searchTerm = !string.IsNullOrWhiteSpace(filter.Codigo) ? filter.Codigo : filter.SearchText;
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedTerm = searchTerm.Trim().ToLower();
                
                // Si es número, buscar solo por código
                if (long.TryParse(normalizedTerm, out _))
                {
                    query = query.Where(x => x.Codigo.Trim().ToLower().Contains(normalizedTerm));
                }
                else
                {
                    query = query.Where(x => 
                        x.Codigo.Trim().ToLower().Contains(normalizedTerm) ||
                        x.NoRegTribut.Trim().ToLower().Contains(normalizedTerm) ||
                        x.Nombre.Trim().ToLower().Contains(normalizedTerm));
                }
            }
            
            return query;
        }
        private IQueryable<MtrCliente> GetClientesPorVendedor(MtrClienteQueryFilter filter, string usuario)
        {
            var query = _context.MtrCliente
                .Where(x => x.Vendedor1 == usuario && x.FlagInactivo != "X");
            
            var searchTerm = !string.IsNullOrWhiteSpace(filter.Codigo) ? filter.Codigo : filter.SearchText;
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedTerm = searchTerm.Trim().ToLower();
                query = query.Where(x => 
                    x.Codigo.Trim().ToLower().Contains(normalizedTerm) ||
                    x.NoRegTribut.Trim().ToLower().Contains(normalizedTerm) ||
                    x.Nombre.Trim().ToLower().Contains(normalizedTerm));
            }
            
            return query;
        }

        private IQueryable<MtrCliente> GetClientesPorSupervisor(MtrClienteQueryFilter filter, string usuario)
        {
            var query = from u in _context.MtrCliente
                        join ur in _context.MtrVendedor on u.Vendedor1 equals ur.Codigo
                        where ur.Supervisor == usuario && u.FlagInactivo != "X"
                        select u;
            
            // Aplicar filtro de búsqueda
            var searchTerm = !string.IsNullOrWhiteSpace(filter.Codigo) ? filter.Codigo : filter.SearchText;
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var normalizedTerm = searchTerm.Trim().ToLower();
                query = query.Where(x => 
                    x.Codigo.Trim().ToLower().Contains(normalizedTerm) ||
                    x.NoRegTribut.Trim().ToLower().Contains(normalizedTerm) ||
                    x.Nombre.Trim().ToLower().Contains(normalizedTerm));
            }
            
            return query;
        }

        public async Task<PagedResult<MtrCliente>> ListClientesPorUsuario(MtrClienteQueryFilter filter)
        {
            IQueryable<MtrCliente> query = null;
            
            // Validar filtros básicos
            if (string.IsNullOrWhiteSpace(filter.Usuario))
                return new PagedResult<MtrCliente> { Items = new List<MtrCliente>(), TotalCount = 0 };
            
            // Determinar el rol del usuario
            var supervisor = await _context.MtrVendedor
                .Where(x => x.Supervisor == filter.Usuario)
                .FirstOrDefaultAsync();
            
            var vendedor = supervisor == null ? await _context.MtrVendedor
                .Where(x => x.Codigo == filter.Usuario)
                .FirstOrDefaultAsync() : null;
            
            var usuarioOficina = await _context.MtrUsuarioOficina
                .Where(x => x.Usuario == filter.Usuario)
                .ToListAsync();
            
            // Construir query según el rol
            if (supervisor != null)
            {
                query = GetClientesPorSupervisor(filter,filter.Usuario);
            }
            else if (vendedor != null)
            {
                query = GetClientesPorVendedor(filter, filter.Usuario);
            }
            else if (usuarioOficina.Any())
            {
                query = GetClientesPorOficina(filter);
            }
            else
            {
                return new PagedResult<MtrCliente> { Items = new List<MtrCliente>(), TotalCount = 0 };
            }
            
            // Aplicar filtros adicionales
            query = ApplyAdditionalFilters(query, filter);
            
            // Obtener total antes de paginar
            var totalCount = await query.CountAsync();
            
            // Aplicar paginación
            var items = await query
                .OrderBy(x => x.Nombre)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
            
            // Agregar cliente prospecto si no está ya incluido
            items = await AddClienteProspecto(items);
            
            return new PagedResult<MtrCliente>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

                

    }

    


    




}
