using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.AppCalculadora;


namespace AppService.Infrastructure.Repositories
{
    public class AppCalculadoraRepository:IAppCalculadoraRepository
    {
        private readonly RRDContext _context;

        public AppCalculadoraRepository(RRDContext context)
        {
            _context = context;
        }

        public async Task<AppCalculadora> GetById(int id)
        {

            AppCalculadora result = new AppCalculadora();
            result = await _context.AppCalculadora
                .Where(a =>  a.Id == id)
                .FirstOrDefaultAsync();
            

            return result;

        }

        public async Task<List<AppCalculadora>> GetByVendedor(AppCalculadoraFilterDto filter)
        {

            List<AppCalculadora> result = new List<AppCalculadora>();
            var vendedores = await _context.MtrVendedor.Where(x => x.Codigo ==filter.IdVendedor).FirstOrDefaultAsync();
            if (vendedores == null)
            {
                if (filter.SearchText.Length > 0)
                {
                    result = await _context.AppCalculadora
                        .Where(a =>  a.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .OrderByDescending(x=>x.Id)
                        .ToListAsync();
                }
                else
                {
                    result = await _context.AppCalculadora
                  
                        .OrderByDescending(x=>x.Id)
                        .ToListAsync();
                }
             
            }
            else
            {
                if (filter.SearchText.Length > 0)
                {
                    result = await _context.AppCalculadora
                        .Where(a => a.IdVendedor == filter.IdVendedor && a.SearchText.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))
                        .OrderByDescending(x=>x.Id)
                        .ToListAsync();
                }
                else
                {
                    result = await _context.AppCalculadora
                        .Where(a => a.IdVendedor == filter.IdVendedor)
                        .OrderByDescending(x=>x.Id)
                        .ToListAsync();
                }
         
            }

            return result;

        }

        public async Task<string> Create(AppCalculadoraUpdateDto dto)
        {
            try
            {
                var cantidad = dto.Cantidad.ToString(CultureInfo.InvariantCulture);
                var cantidad_X_Unidad = dto.Cantidad_X_Unidad.ToString(CultureInfo.InvariantCulture);
                var cantidadConvertida = dto.CantidadConvertida.ToString(CultureInfo.InvariantCulture);
                var precioMinimo = dto.PrecioMinimo.ToString(CultureInfo.InvariantCulture);
                var precioMaximo = dto.PrecioMaximo.ToString(CultureInfo.InvariantCulture);
                var precio = dto.Precio.ToString(CultureInfo.InvariantCulture);
                var total = dto.Total.ToString(CultureInfo.InvariantCulture);
                var porcFlete = dto.PorcFlete.ToString(CultureInfo.InvariantCulture);
                var fecha = $"{DateTime.Now.Day}/{DateTime.Now.Month}/{DateTime.Now.Year}";
                var searchText =
                    $"{dto.IdVendedor}-{dto.DescripcionMunicipio}-{dto.DescripcionCondPago}-{dto.DescripcionTipoMonedaPago}-{dto.DescripcionSubcategoria}-{dto.DescripcionProducto}-{dto.DescripcionUnidad}-{dto.MedidaBasica}-{dto.MedidaOpuesta}-{dto.CalculoId}-{dto.IdCliente}-{dto.RazonSocial}-{fecha}";
                /*
                 * @Id INT,
                      @IdVendedor NVARCHAR(50),
                      @IdMunicipio INT,
                      @DescripcionMunicipio NVARCHAR(255),
                      @IdCondPago INT,
                      @DescripcionCondPago NVARCHAR(255),
                      @IdMtrTipoMonedaPago INT,
                      @DescripcionTipoMonedaPAgo NVARCHAR(255),
                      @IdSubcategoria INT,
                      @DescripcionSubcategoria NVARCHAR(255),
                      @IdProducto INT,
                      @DescripcionProducto NVARCHAR(255),
                      @Cantidad DECIMAL(18, 2),
                      @IdUnidad INT,
                      @DescripcionUnidad NVARCHAR(255),
                      @Cantidad_X_Unidad DECIMAL(18, 2),
                      @CantidadConvertida DECIMAL(18, 2),
                      @MedidaBasica NVARCHAR(50),
                      @MedidaOpuesta NVARCHAR(50),
                      @PrecioMinimo DECIMAL(18, 2),
                      @PrecioMaximo DECIMAL(18, 2),
                      @Precio DECIMAL(18, 2),
                      @Total DECIMAL(18, 2),
                      @CalculoId INT = NULL,
                      @UserUpdate NVARCHAR(50),
                      @SearchText NVARCHAR(MAX) = NULL
                 * 
                 */
                
                
                FormattableString xqueryDiarioPasePlanta = $"exec rrd.dbo.InsertAppCalculadora {dto.IdVendedor},{dto.IdMunicipio},{dto.DescripcionMunicipio},{dto.IdCondPago},{dto.DescripcionCondPago},{dto.IdMtrTipoMonedaPago},{dto.DescripcionTipoMonedaPago},{dto.IdSubcategoria},{dto.DescripcionSubcategoria},{dto.IdProducto},{dto.DescripcionProducto},{cantidad},{dto.IdUnidad},{dto.DescripcionUnidad},{cantidad_X_Unidad},{cantidadConvertida},{dto.MedidaBasica},{dto.MedidaOpuesta},{precioMinimo},{precioMaximo},{precio},{total},{dto.CalculoId},{dto.UserUpdate},{searchText},{dto.IdCliente},{dto.IdDireccionEntregar},{dto.RazonSocial},{porcFlete}";
                await _context.Database.ExecuteSqlInterpolatedAsync(xqueryDiarioPasePlanta);
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               return e.Message;
            }
            
        
        }
        
        
        public async Task<string> Update(AppCalculadoraUpdateDto dto)
        {
            try
            {
                var cantidad = dto.Cantidad.ToString(CultureInfo.InvariantCulture);
                var cantidad_X_Unidad = dto.Cantidad_X_Unidad.ToString(CultureInfo.InvariantCulture);
                var cantidadConvertida = dto.CantidadConvertida.ToString(CultureInfo.InvariantCulture);
                var precioMinimo = dto.PrecioMinimo.ToString(CultureInfo.InvariantCulture);
                var precioMaximo = dto.PrecioMaximo.ToString(CultureInfo.InvariantCulture);
                var precio = dto.Precio.ToString(CultureInfo.InvariantCulture);
                var total = dto.Total.ToString(CultureInfo.InvariantCulture);
                var porcFlete = dto.PorcFlete.ToString(CultureInfo.InvariantCulture);
                var fecha = $"{dto.CreatedAt.Day}/{dto.CreatedAt.Month}/{dto.CreatedAt.Year}";
                var searchText =
                    $"{dto.IdVendedor}-{dto.DescripcionMunicipio}-{dto.DescripcionCondPago}-{dto.DescripcionTipoMonedaPago}-{dto.DescripcionSubcategoria}-{dto.DescripcionProducto}-{dto.DescripcionUnidad}-{dto.MedidaBasica}-{dto.MedidaOpuesta}-{dto.CalculoId}-{dto.IdCliente}-{dto.RazonSocial}-{fecha}";
                
                /*
                 * @Id INT,
                      @IdVendedor NVARCHAR(50),
                      @IdMunicipio INT,
                      @DescripcionMunicipio NVARCHAR(255),
                      @IdCondPago INT,
                      @DescripcionCondPago NVARCHAR(255),
                      @IdMtrTipoMonedaPago INT,
                      @DescripcionTipoMonedaPAgo NVARCHAR(255),
                      @IdSubcategoria INT,
                      @DescripcionSubcategoria NVARCHAR(255),
                      @IdProducto INT,
                      @DescripcionProducto NVARCHAR(255),
                      @Cantidad DECIMAL(18, 2),
                      @IdUnidad INT,
                      @DescripcionUnidad NVARCHAR(255),
                      @Cantidad_X_Unidad DECIMAL(18, 2),
                      @CantidadConvertida DECIMAL(18, 2),
                      @MedidaBasica NVARCHAR(50),
                      @MedidaOpuesta NVARCHAR(50),
                      @PrecioMinimo DECIMAL(18, 2),
                      @PrecioMaximo DECIMAL(18, 2),
                      @Precio DECIMAL(18, 2),
                      @Total DECIMAL(18, 2),
                      @CalculoId INT = NULL,
                      @UserUpdate NVARCHAR(50),
                      @SearchText NVARCHAR(MAX) = NULL
                 * 
                 */
                
                
                FormattableString xqueryDiarioPasePlanta = $"exec rrd.dbo.UpdateAppCalculadora {dto.Id},{dto.IdVendedor},{dto.IdMunicipio},{dto.DescripcionMunicipio},{dto.IdCondPago},{dto.DescripcionCondPago},{dto.IdMtrTipoMonedaPago},{dto.DescripcionTipoMonedaPago},{dto.IdSubcategoria},{dto.DescripcionSubcategoria},{dto.IdProducto},{dto.DescripcionProducto},{cantidad},{dto.IdUnidad},{dto.DescripcionUnidad},{cantidad_X_Unidad},{cantidadConvertida},{dto.MedidaBasica},{dto.MedidaOpuesta},{precioMinimo},{precioMaximo},{precio},{total},{dto.CalculoId},{dto.UserUpdate},{searchText},{dto.IdCliente},{dto.IdDireccionEntregar},{dto.RazonSocial},{porcFlete}";
                await _context.Database.ExecuteSqlInterpolatedAsync(xqueryDiarioPasePlanta);
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               return e.Message;
            }
            
        
        }
        
        public async Task<string> Delete(int  id)
        {
            try
            {
             
                
                FormattableString xqueryDiarioPasePlanta = $"exec rrd.dbo.DeleteAppCalculadora {id}";
                await _context.Database.ExecuteSqlInterpolatedAsync(xqueryDiarioPasePlanta);
                return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
            
        
        }


    }
}
