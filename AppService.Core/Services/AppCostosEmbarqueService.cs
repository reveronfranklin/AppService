using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static iText.IO.Util.IntHashtable;

namespace AppService.Core.Services
{
    public class AppCostosEmbarqueService : IAppCostosEmbarqueService
    {


        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppCostosEmbarqueService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<AppCostosEmbarque>> GetAll()
        {

            List<AppCostosEmbarque> appConfigApp = await _unitOfWork.AppCostosEmbaqueRepository.GetAll();

            return appConfigApp;
        }





        public async Task<AppCostosEmbarque> GetById(decimal id)
        {

            var costoEmbarque = await _unitOfWork.AppCostosEmbaqueRepository.GetById(id);

            // AppConfigAppGetDto appConfigDto = _mapper.Map<AppConfigAppGetDto>(appConfig);

            return costoEmbarque;
        }
        public async Task<AppCostosEmbarque> Update(AppCostosEmbarque entity)
        {

            var AppConfigAppFind = await GetById(entity.Id);
            if (AppConfigAppFind == null)
            {
                throw new Exception("Documento No existe");
            }

            _unitOfWork.AppCostosEmbaqueRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return await GetById(entity.Id);


        }







        public async Task<AppCostosEmbarque> Insert(AppCostosEmbarque entity)
        {


            await _unitOfWork.AppCostosEmbaqueRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            return entity;


        }


        public async Task CrearCostosDelMes(int ano, int mes,string tipoDocumento="FA")
        {
            if(ano == 0 && mes == 0)
            {
                var winy244 = await _unitOfWork.Winy244Repository.GetUltimoPeriodo();
                ano = (int)winy244.AñoContable;
                mes = (int)winy244.MesContable;
            }

           await _unitOfWork.AppCostosEmbaqueRepository.DeleteRange(ano, mes, tipoDocumento);
           await _unitOfWork.SaveChangesAsync();

            if (tipoDocumento == "FA")
            {
                var facturas = await _unitOfWork.Ciny057Repository.GetByPeriodo(ano, mes);
                if (facturas.Count > 0)
                {
                    foreach (var item in facturas)
                    {
                        var factura = await _unitOfWork.Ciny056Repository.GetByNumero(item.Factura);
                        var codCliente = (int)factura.Cliente;
                        var clienteString = codCliente.ToString();
                        var cliente = await _unitOfWork.MtrClienteRepository.GetByIdAsync(clienteString);
                        AppCostosEmbarque itemCostoEmbarque = new AppCostosEmbarque();
                        itemCostoEmbarque.TipoDocumento = "FA";
                        itemCostoEmbarque.Documento = item.Factura;
                        itemCostoEmbarque.DescripcionDocumento = "FACTURA";
                        itemCostoEmbarque.Fecha = item.FechaFactura;
                        itemCostoEmbarque.Ano = item.Año;
                        itemCostoEmbarque.Mes = item.Mes;
                        itemCostoEmbarque.Monto = item.ValorVentad;
                        itemCostoEmbarque.Signo = 1;
                        itemCostoEmbarque.Cliente = factura.Cliente;
                        itemCostoEmbarque.NombreCliente = factura.Nombre;
                        itemCostoEmbarque.Mes = item.Mes;
                        itemCostoEmbarque.CodVendedor = cliente.Vendedor1;
                        var vendedor = await _unitOfWork.MtrVendedorRepository.GetByIdAsync(itemCostoEmbarque.CodVendedor);
                        itemCostoEmbarque.NombreVededor = vendedor.Nombre;
                        itemCostoEmbarque.CodOficina = vendedor.Oficina;
                        var oficina = await _unitOfWork.MtrOficinaRepository.GetById(vendedor.Oficina);
                        itemCostoEmbarque.NombreOficina = oficina.NomOficina;
                        itemCostoEmbarque.CodRegion = vendedor.CodigoRegion;

                        itemCostoEmbarque.NombreRegion = "";
                        itemCostoEmbarque.Orden = item.Orden;
                        itemCostoEmbarque.Articulo = item.Articulo;
                        itemCostoEmbarque.Titulo = "EMBARQUE";
                        itemCostoEmbarque.CodeVariable = "EMBARQUE";
                        itemCostoEmbarque.Valor = item.ValorVentad;
                        itemCostoEmbarque.ValorFacturado = item.ValorVentad;
                        itemCostoEmbarque.OrigenCotizacion = "SIN ORDEN";
                        if (item.Orden == 0)
                        {
                            await _unitOfWork.AppCostosEmbaqueRepository.Add(itemCostoEmbarque);
                            await _unitOfWork.SaveChangesAsync();
                        }
                        else
                        {
                            var cotizacion = await _unitOfWork.PropuestaRepository.GetByOrden(item.Orden);
                            if (cotizacion != null)
                            {
                                itemCostoEmbarque.VentaCotizacion = (decimal)cotizacion.TotalPropuestaUsd;
                                itemCostoEmbarque.VentaOrden = (decimal)cotizacion.TotalPropuestaUsd;
                                itemCostoEmbarque.PoncertajeFacturado = ((decimal)itemCostoEmbarque.Monto * 100) / (decimal)itemCostoEmbarque.VentaOrden;

                                if (cotizacion.CantidadExterno > 0)
                                {
                                    itemCostoEmbarque.OrigenCotizacion = "GENERADOR PRESUPUESTO";
                                }
                                else
                                {
                                    itemCostoEmbarque.OrigenCotizacion = "VIEJO COTIZADOR";
                                }
                                itemCostoEmbarque.Cotizacion = cotizacion.Cotizacion;
                                var generalQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetByCotizacions(cotizacion.Cotizacion);
                                if (generalQuotes != null)
                                {
                                    itemCostoEmbarque.AppGeneralQuotesId = generalQuotes.Id;
                                    var detailQuotes = await _unitOfWork.AppDetailQuotesRepository.GetByGeneralQuotesExternalcode(generalQuotes.Id, item.Articulo);
                                    if (detailQuotes != null)
                                    {
                                        itemCostoEmbarque.AppDetailQuotesId = detailQuotes.Id;
                                        itemCostoEmbarque.CalculoId = detailQuotes.CalculoId;
                                        itemCostoEmbarque.AppProductsId = detailQuotes.IdProducto;
                                        var producto = await _unitOfWork.AppProductsRepository.GetById(detailQuotes.IdProducto);
                                        itemCostoEmbarque.IdSubCategoria = producto.AppSubCategoryId;
                                    }
                                }


                                itemCostoEmbarque.Valor = item.ValorVentad;
                                itemCostoEmbarque.ValorFacturado = (itemCostoEmbarque.Valor * itemCostoEmbarque.PoncertajeFacturado) / 100;
                                await _unitOfWork.AppCostosEmbaqueRepository.Add(itemCostoEmbarque);
                                await _unitOfWork.SaveChangesAsync();


                                if (itemCostoEmbarque.IdSubCategoria != null && itemCostoEmbarque.IdSubCategoria > 0 && itemCostoEmbarque.CalculoId > 0)
                                {
                                    var titulos = await _unitOfWork.AppTituloVariableRepository.GetAllBySubCategoria((int)itemCostoEmbarque.IdSubCategoria);
                                    if (titulos.Count > 0)
                                    {
                                        foreach (var itemTitulos in titulos)
                                        {
                                            var calculo = await _unitOfWork.AppRecipesByAppDetailQuotesRepository.GetByCalculoIdCode((int)itemCostoEmbarque.CalculoId, itemTitulos.CodeVariable.Trim());
                                            if (calculo != null)
                                            {
                                                AppCostosEmbarque newItemCostoEmbarque = new AppCostosEmbarque();
                                                newItemCostoEmbarque = itemCostoEmbarque;
                                                newItemCostoEmbarque.Signo = 1;
                                                newItemCostoEmbarque.Id = 0;
                                                newItemCostoEmbarque.Titulo = itemTitulos.Titulo;
                                                newItemCostoEmbarque.CodeVariable = itemTitulos.CodeVariable;
                                                if (calculo.TotalCost > 0)
                                                {
                                                    newItemCostoEmbarque.Valor = calculo.TotalCost;
                                                }
                                                else
                                                {
                                                    newItemCostoEmbarque.Valor = calculo.Quantity;
                                                }

                                                newItemCostoEmbarque.VentaCotizacion = itemCostoEmbarque.VentaCotizacion;
                                                newItemCostoEmbarque.VentaCotizacion = itemCostoEmbarque.VentaOrden;
                                                newItemCostoEmbarque.PoncertajeFacturado = itemCostoEmbarque.PoncertajeFacturado;
                                                newItemCostoEmbarque.ValorFacturado = (newItemCostoEmbarque.Valor * newItemCostoEmbarque.PoncertajeFacturado) / 100;
                                                await _unitOfWork.AppCostosEmbaqueRepository.Add(newItemCostoEmbarque);
                                                await _unitOfWork.SaveChangesAsync();
                                            }

                                        }
                                    }
                                }


                            }
                        }
                           



                    }



                }


            }

            if (tipoDocumento == "NC")
            {
                var notas = await _unitOfWork.Ciny057Repository.GetNotasCreditoDetalleByPeriodo(ano, mes);
                if (notas.Count > 0)
                {
                    foreach (var item in notas)
                    {
                        var factura = await _unitOfWork.Ciny057Repository.GetNotaCreditoByNumero(item.NumeroNota);
                        var codCliente = (int)factura.Cliente;
                        var clienteString = codCliente.ToString();
                        var cliente = await _unitOfWork.MtrClienteRepository.GetByIdAsync(clienteString);
                        AppCostosEmbarque itemCostoEmbarque = new AppCostosEmbarque();
                        itemCostoEmbarque.TipoDocumento = "NC";
                        itemCostoEmbarque.Documento = item.NumeroNota;
                        itemCostoEmbarque.DescripcionDocumento = "NOTA DE CREDITO";
                        itemCostoEmbarque.Fecha = item.FechaNota;
                        itemCostoEmbarque.Ano = item.Año;
                        itemCostoEmbarque.Mes = item.Mes;
                        var tasa = await _unitOfWork.TPaTasaBancoCentralRepository.GetTasaByFecha(item.FechaNota);

                        itemCostoEmbarque.Monto = item.ValorVenta / tasa.Tasa;
                        
                        itemCostoEmbarque.Monto = Decimal.Round((decimal)itemCostoEmbarque.Monto, 2); 
                        item.ValorVentad = (decimal)itemCostoEmbarque.Monto;
                       


                        itemCostoEmbarque.Signo = -1;
                        itemCostoEmbarque.Cliente = factura.Cliente;
                        itemCostoEmbarque.NombreCliente = factura.NombreCliente;
                        itemCostoEmbarque.Mes = item.Mes;
                        itemCostoEmbarque.CodVendedor = cliente.Vendedor1;
                        var vendedor = await _unitOfWork.MtrVendedorRepository.GetByIdAsync(itemCostoEmbarque.CodVendedor);
                        itemCostoEmbarque.NombreVededor = vendedor.Nombre;
                        itemCostoEmbarque.CodOficina = vendedor.Oficina;
                        var oficina = await _unitOfWork.MtrOficinaRepository.GetById(vendedor.Oficina);
                        itemCostoEmbarque.NombreOficina = oficina.NomOficina;
                        itemCostoEmbarque.CodRegion = vendedor.CodigoRegion;

                        itemCostoEmbarque.NombreRegion = "";

                        var orden = long.Parse(item.Orden);
                        itemCostoEmbarque.Orden = orden;
                        itemCostoEmbarque.Articulo = item.CodProducto;
                        var cotizacion = await _unitOfWork.PropuestaRepository.GetByOrden(orden);
                        if (cotizacion != null)
                        {
                            itemCostoEmbarque.VentaCotizacion = (decimal)cotizacion.TotalPropuestaUsd;
                            itemCostoEmbarque.VentaOrden = (decimal)cotizacion.TotalPropuestaUsd;
                            itemCostoEmbarque.PoncertajeFacturado = ((decimal)itemCostoEmbarque.Monto * 100) / (decimal)itemCostoEmbarque.VentaOrden;

                            if (cotizacion.CantidadExterno > 0)
                            {
                                itemCostoEmbarque.OrigenCotizacion = "GENERADOR PRESUPUESTO";
                            }
                            else
                            {
                                itemCostoEmbarque.OrigenCotizacion = "VIEJO COTIZADOR";
                            }
                            itemCostoEmbarque.Cotizacion = cotizacion.Cotizacion;
                            var generalQuotes = await _unitOfWork.AppGeneralQuotesRepository.GetByCotizacions(cotizacion.Cotizacion);
                            if (generalQuotes != null)
                            {
                                itemCostoEmbarque.AppGeneralQuotesId = generalQuotes.Id;
                                var detailQuotes = await _unitOfWork.AppDetailQuotesRepository.GetByGeneralQuotesExternalcode(generalQuotes.Id, item.CodProducto);
                                if (detailQuotes != null)
                                {
                                    itemCostoEmbarque.AppDetailQuotesId = detailQuotes.Id;
                                    itemCostoEmbarque.CalculoId = detailQuotes.CalculoId;
                                    itemCostoEmbarque.AppProductsId = detailQuotes.IdProducto;
                                    var producto = await _unitOfWork.AppProductsRepository.GetById(detailQuotes.IdProducto);
                                    itemCostoEmbarque.IdSubCategoria = producto.AppSubCategoryId;
                                }
                            }

                            itemCostoEmbarque.Titulo = "EMBARQUE";
                            itemCostoEmbarque.CodeVariable = "EMBARQUE";
                            itemCostoEmbarque.Valor = item.ValorVentad;
                            itemCostoEmbarque.ValorFacturado = itemCostoEmbarque.Valor;


                            await _unitOfWork.AppCostosEmbaqueRepository.Add(itemCostoEmbarque);
                            await _unitOfWork.SaveChangesAsync();
                            if (itemCostoEmbarque.IdSubCategoria != null && itemCostoEmbarque.IdSubCategoria > 0 && itemCostoEmbarque.CalculoId > 0)
                            {
                                var titulos = await _unitOfWork.AppTituloVariableRepository.GetAllBySubCategoria((int)itemCostoEmbarque.IdSubCategoria);
                                if (titulos.Count > 0)
                                {
                                    foreach (var itemTitulos in titulos)
                                    {
                                        var calculo = await _unitOfWork.AppRecipesByAppDetailQuotesRepository.GetByCalculoIdCode((int)itemCostoEmbarque.CalculoId, itemTitulos.CodeVariable.Trim());
                                        if (calculo != null)
                                        {
                                            AppCostosEmbarque newItemCostoEmbarque = new AppCostosEmbarque();
                                            newItemCostoEmbarque = itemCostoEmbarque;
                                            newItemCostoEmbarque.Signo = -1;
                                            newItemCostoEmbarque.Id = 0;
                                            newItemCostoEmbarque.Titulo = itemTitulos.Titulo;
                                            newItemCostoEmbarque.CodeVariable = itemTitulos.CodeVariable;
                                            if (calculo.TotalCost > 0)
                                            {
                                                newItemCostoEmbarque.Valor = calculo.TotalCost;
                                            }
                                            else
                                            {
                                                newItemCostoEmbarque.Valor = calculo.Quantity;
                                            }

                                            newItemCostoEmbarque.VentaCotizacion = itemCostoEmbarque.VentaCotizacion;
                                            newItemCostoEmbarque.VentaCotizacion = itemCostoEmbarque.VentaOrden;
                                            newItemCostoEmbarque.PoncertajeFacturado = itemCostoEmbarque.PoncertajeFacturado;
                                            newItemCostoEmbarque.ValorFacturado = (newItemCostoEmbarque.Valor * newItemCostoEmbarque.PoncertajeFacturado) / 100;

                                            newItemCostoEmbarque.Valor = Decimal.Round((decimal)newItemCostoEmbarque.Valor, 2);
                                            newItemCostoEmbarque.ValorFacturado = Decimal.Round((decimal)newItemCostoEmbarque.ValorFacturado, 2);

                                            await _unitOfWork.AppCostosEmbaqueRepository.Add(newItemCostoEmbarque);
                                            await _unitOfWork.SaveChangesAsync();
                                        }

                                    }
                                }
                            }


                        }



                    }



                }


            }


        }

        public async Task<bool> Delete(decimal id)
        {
            await _unitOfWork.AppCostosEmbaqueRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }



    }

    
}
