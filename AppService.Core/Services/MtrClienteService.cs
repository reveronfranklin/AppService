using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Odoo.Clientes;
using AppService.Core.Entities;
using AppService.Core.EntitiesClientes;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Core.Responses;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Core.Services
{
    public class MtrClienteService : IMtrClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOptions _paginationOptions;
        private readonly IMapper _mapper;
        private readonly ICotizacionService _cotizacionService;

        public MtrClienteService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options, IMapper mapper,ICotizacionService  cotizacionService)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _mapper = mapper;
            _cotizacionService = cotizacionService;
        }


        public async Task<IEnumerable<MtrCliente>> ListClientesPorUsuario(MtrClienteQueryFilter filter)
        {

            filter.PageNumber = filter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filter.PageNumber;
            filter.PageSize = filter.PageSize == 0 ? _paginationOptions.DefaultPageSize : filter.PageSize;

            var clientes = await _unitOfWork.MtrClienteRepository.ListClientesPorUsuario(filter);



            return clientes;

        }

        public async Task OdooActualizarClientes( )
        {

            //await _cotizacionService.UpdateCotizacionesToOdoo();

            await _cotizacionService.ActualizarClientes();



        }




        public async Task<MtrCliente> GetByIdAsync(string id)
        {
            return await _unitOfWork.MtrClienteRepository.GetByIdAsync(id);
            
        }

        public MtrCliente GetById(string id)
        {
            return _unitOfWork.MtrClienteRepository.GetById(id);
        }
        
        public async Task<ApiResponse<bool>> UpdateDireccionCliente(MtrClienteDireccionUpdateDto dto)
        {
     
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<bool> response = new ApiResponse<bool>(false);

            
            var cliente = await GetByIdAsync(dto.Codigo);
            if (cliente == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Cliente No existe";
                response.Data = false;
                response.Meta = metadata;
                return response;
            }
            decimal sectorDecimal = decimal.Parse(dto.Sector);
            var sector = await _unitOfWork.Wsmy065Repository.GetSectorBySector(sectorDecimal);
            if (sector != null)
            {
                if (sector.FlagInactiva == true)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Sector Invalido";
                    response.Data = false;
                    response.Meta = metadata;
                    return response;
                }
            }
            else
            {
                metadata.IsValid = false;
                metadata.Message = "Sector Invalido";
                response.Data = false;
                response.Meta = metadata;
                return response;
            }
            decimal ramoDecimal = decimal.Parse(dto.Ramo);
            var ramo = await _unitOfWork.Wsmy065Repository.GetByRamo(ramoDecimal);
            if (ramo != null)
            {
                if (ramo.FlagInactiva == true)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Ramo Invalido";
                    response.Data = false;
                    response.Meta = metadata;
                    return response;
                }
               
                                
            } else
            {
                metadata.IsValid = false;
                metadata.Message = "Ramo Invalido";
                response.Data = false;
                response.Meta = metadata;
                return response;
                                    
            }
            if (dto.Direccion1 == null) dto.Direccion1 = "";
            if (dto.Direccion2 == null) dto.Direccion2 = "";
            if (dto.Direccion2 == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Direccion Invalida";
                response.Data = false;
                response.Meta = metadata;
                return response;
            }
            
          
            dto.IdDireccionCliente = (decimal)cliente.IdDireccion;
            cliente.TipoNegocio = dto.IdTipoNegocio;
            cliente.Segmento = dto.Sector;
            cliente.SubSegmentoa = dto.Ramo;
            cliente.FModificacion=DateTime.Now.Date;
            _unitOfWork.MtrClienteRepository.Update(cliente);
            await _unitOfWork.SaveChangesAsync();
            
            var csmy003 = await _unitOfWork.Wsmy065Repository.GetClienteCsmy003(dto.Codigo);
            if (csmy003 != null)
            {
                csmy003.TipoNegocio = dto.IdTipoNegocio;
                csmy003.Segmento = dto.Sector;
                csmy003.SubSegmentoa = dto.Ramo;
                csmy003.FModificacion=DateTime.Now.Date;
                _unitOfWork.Wsmy065Repository.UpdateCsmy003(csmy003);
                await _unitOfWork.SaveChangesAsync();
                
            }

            if (dto.Id != dto.IdDireccionCliente)
            {
                var wsmy264 = await _unitOfWork.Wsmy264Repository.GetById(dto.Id);
                if (wsmy264 != null)
                {
                    wsmy264.Rif = dto.RifDireccion;
                    wsmy264.Direccion = dto.Direccion;
                    wsmy264.Direccion1 = dto.Direccion1;
                    wsmy264.Direccion2 = dto.Direccion2;
                    wsmy264.Estado = dto.Estado;
                    wsmy264.Municipio = dto.Municipio;
                    wsmy264.FechaUpdate=DateTime.Now.Date;
                    wsmy264.PuntoReferencia = dto.PuntoReferencia;
                    wsmy264.Origen = dto.Usuario;
                    _unitOfWork.Wsmy264Repository.UpdateQuery(wsmy264);
                    //await _unitOfWork.SaveChangesAsync();

                }
                
                var direccion = await _unitOfWork.MtrDireccionesRepository.GetById(dto.Id);
                if (direccion != null)
                {
                    direccion.Rif = dto.RifDireccion;
                    direccion.Direccion = dto.Direccion;
                    direccion.Direccion1 = dto.Direccion1;
                    direccion.Direccion2 = dto.Direccion2;
                    direccion.FechaUpdate=DateTime.Now.Date;
                    direccion.Estado = dto.Estado;
                    direccion.Municipio = dto.Municipio;
                    direccion.PuntoReferencia = dto.PuntoReferencia;
                    direccion.Origen = dto.Usuario;
                    _unitOfWork.MtrDireccionesRepository.Update(direccion);
                    await _unitOfWork.SaveChangesAsync();
                }

                
            }



            metadata.Message = "Actualizado Satisfactoriamente";
            metadata.success = true;
            response.Meta = metadata;
            return response;
        }
       
        public async Task<ApiResponse<bool>> CreateDireccionCliente(MtrClienteDireccionUpdateDto dto)
        {
     
            Metadata metadata = new Metadata()
            {
                IsValid = true,
                Message = ""
            };
            ApiResponse<bool> response = new ApiResponse<bool>(false);

            
            var cliente = await GetByIdAsync(dto.Codigo);
            if (cliente == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Cliente No existe";
                response.Data = false;
                response.Meta = metadata;
                return response;
            }
            decimal sectorDecimal = decimal.Parse(dto.Sector);
            var sector = await _unitOfWork.Wsmy065Repository.GetSectorBySector(sectorDecimal);
            if (sector != null)
            {
                if (sector.FlagInactiva == true)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Sector Invalido";
                    response.Data = false;
                    response.Meta = metadata;
                    return response;
                }
            }
            else
            {
                metadata.IsValid = false;
                metadata.Message = "Sector Invalido";
                response.Data = false;
                response.Meta = metadata;
                return response;
            }
            decimal ramoDecimal = decimal.Parse(dto.Ramo);
            var ramo = await _unitOfWork.Wsmy065Repository.GetByRamo(ramoDecimal);
            if (ramo != null)
            {
                if (ramo.FlagInactiva == true)
                {
                    metadata.IsValid = false;
                    metadata.Message = "Ramo Invalido";
                    response.Data = false;
                    response.Meta = metadata;
                    return response;
                }
               
                                
            } else
            {
                metadata.IsValid = false;
                metadata.Message = "Ramo Invalido";
                response.Data = false;
                response.Meta = metadata;
                return response;
                                    
            }
            if (dto.Direccion1 == null) dto.Direccion1 = "";
            if (dto.Direccion2 == null) dto.Direccion2 = "";
            if (dto.Direccion2 == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Direccion Invalida";
                response.Data = false;
                response.Meta = metadata;
                return response;
            }
            
          
            dto.IdDireccionCliente = (decimal)cliente.IdDireccion;
            cliente.TipoNegocio = dto.IdTipoNegocio;
            cliente.Segmento = dto.Sector;
            cliente.SubSegmentoa = dto.Ramo;
            cliente.FModificacion=DateTime.Now.Date;
            _unitOfWork.MtrClienteRepository.Update(cliente);
            await _unitOfWork.SaveChangesAsync();
            
            var csmy003 = await _unitOfWork.Wsmy065Repository.GetClienteCsmy003(dto.Codigo);
            if (csmy003 != null)
            {
                csmy003.TipoNegocio = dto.IdTipoNegocio;
                csmy003.Segmento = dto.Sector;
                csmy003.SubSegmentoa = dto.Ramo;
                csmy003.FModificacion=DateTime.Now.Date;
                _unitOfWork.Wsmy065Repository.UpdateCsmy003(csmy003);
                await _unitOfWork.SaveChangesAsync();
                
            }

            Wsmy264 wsmy264New = new Wsmy264();
            decimal codigoDecimal = decimal.Parse(dto.Codigo);
            wsmy264New.Id = 0; 
            wsmy264New.IdDireccion = await _unitOfWork.Wsmy264Repository.NextId(codigoDecimal);
            wsmy264New.Codigo = codigoDecimal;
            wsmy264New.Rif = dto.RifDireccion.Trim();
            wsmy264New.Direccion = dto.Direccion;
            wsmy264New.Direccion1 = dto.Direccion1;
            wsmy264New.Direccion2 = dto.Direccion2;
            wsmy264New.Estado = dto.Estado;
            wsmy264New.Municipio = dto.Municipio;
            wsmy264New.FechaUpdate=DateTime.Now.Date;
            wsmy264New.PuntoReferencia = dto.PuntoReferencia;
            wsmy264New.Origen = dto.Usuario;
            _unitOfWork.Wsmy264Repository.AddQuery(wsmy264New);
            var wsmy264Find = await _unitOfWork.Wsmy264Repository.GetByCodigoIdDireccion(wsmy264New.IdDireccion,codigoDecimal );
            if (wsmy264Find != null)
            {
                var direccion = await _unitOfWork.MtrDireccionesRepository.GetById(wsmy264Find.Id);
                if (direccion == null)
                {
                    MtrDirecciones newDireccion = new MtrDirecciones();
                    newDireccion.Id = wsmy264Find.Id;
                    newDireccion.Codigo = dto.Codigo;
                    newDireccion.Rif = dto.RifDireccion;
                    newDireccion.Direccion = dto.Direccion;
                    newDireccion.Direccion1 = dto.Direccion1;
                    newDireccion.Direccion2 = dto.Direccion2;
                    newDireccion.FechaUpdate=DateTime.Now.Date;
                    newDireccion.Estado = dto.Estado;
                    newDireccion.Municipio = dto.Municipio;
                    newDireccion.PuntoReferencia = dto.PuntoReferencia;
                    newDireccion.Origen = dto.Usuario;
                    newDireccion.Recnum = wsmy264Find.Recnum;
                    _unitOfWork.MtrDireccionesRepository.Add(newDireccion);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

            metadata.Message = "Actualizado Satisfactoriamente";
            metadata.success = true;
            response.Meta = metadata;
            return response;
        }
        public async Task<List<MtrDirecciones>> GetDireccionestByCodigo(string codigo)
        {
            var listDirecciones = await _unitOfWork.MtrDireccionesRepository.GetByIdCliente(codigo);


            return listDirecciones;
        }

        public async Task<List<MtrDireccionesDto>> GetDireccionestDtoByCodigo(string codigo)
        {
            var direcciones = await GetDireccionestByCodigo(codigo);
            direcciones = direcciones.Where(x => x.Inactivo == false).ToList();
            List<MtrDireccionesDto> result = new List<MtrDireccionesDto>();

            if (direcciones != null)
            {
                result = _mapper.Map<List<MtrDireccionesDto>>(direcciones);
            }

            foreach (var item in result)
            {
                var municipio = await _unitOfWork.Winy243Repository.GetByEstadoMunicipio(item.Estado.Trim(), item.Municipio.Trim());
                if (municipio != null)
                {
                    item.IdMunicipio = municipio.Recnum;
                    item.NombreMunicipio = municipio.CapitalMcpo;
                    item.Estado = item.Estado.Trim();
                    item.Municipio=item.Municipio.Trim();
                    if (municipio.PorcFlete == null)
                    {
                        item.PorcFlete = 0;
                    }
                    else
                    {
                        item.PorcFlete = (decimal)municipio.PorcFlete;
                    }
                
                }
  
            }

            return result;

        }

        public async Task<List<MtrSectorDto>> ListSectores()
        {
            List<MtrSectorDto> result = new List<MtrSectorDto>();

            var sectores = await _unitOfWork.Wsmy065Repository.GetListSector();
            if (sectores!=null && sectores.Count>0)
            {
                foreach (var item in sectores.OrderBy(x=>x.NombreSector).ToList())
                {
                    MtrSectorDto resultItem = new MtrSectorDto();
                    resultItem.Sector = item.Sector;
                    resultItem.DescripcionSector = item.NombreSector;
                    List<MtrRamoDto> listRamo = new List<MtrRamoDto>();
                    var ramos  = await _unitOfWork.Wsmy065Repository.GetListRamosBySector(item.Sector);
                    if (ramos.Count > 0)
                    {
                        
                        foreach (var ramosItem in ramos.OrderBy(x=>x.NombreRamo).ToList())
                        {
                            MtrRamoDto ramo = new MtrRamoDto();
                            ramo.Ramo = ramosItem.Ramo;
                            ramo.DescripcionRamo = ramosItem.NombreRamo;
                            listRamo.Add(ramo);
                        }
                        
                    }

                    resultItem.Ramo = listRamo;
                    result.Add(resultItem);
                }
            }


            return result;
        }
        public async Task<List<MtrClienteDireccionDto>> ListDireccionesPorUsuario(MtrClienteQueryFilter filter)
        {

            List<MtrClienteDireccionDto> result = new List<MtrClienteDireccionDto>();
            filter.PageNumber = filter.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filter.PageNumber;
            filter.PageSize = filter.PageSize == 0 ? _paginationOptions.DefaultPageSize : filter.PageSize;

            var clientes = await _unitOfWork.MtrClienteRepository.ListClientesPorUsuario(filter);
            if (clientes.Count() > 0)
            {
                foreach (var item in clientes)
                {
                   
                    
                    var cliente = item.Codigo.Trim();
                    
                    try
                    {
                        if (item.Segmento == null || item.Segmento.Trim()== ""  )
                        {
                            item.Segmento = "0";
                                
                        }
                        if (item.SubSegmentoa == null || item.SubSegmentoa.Trim()== ""  )
                        {
                            item.SubSegmentoa = "0";
                                
                        } 
                    MtrSectorDto sectorObj = new MtrSectorDto();
                    var sectores = await ListSectores();
                    if (sectores.Count > 0)
                    {
                        decimal sectorDecimal = decimal.Parse(item.Segmento);
                        sectorObj = sectores.Where(x => x.Sector ==sectorDecimal).FirstOrDefault();
                        
                    }

                    
                    var status = "";
                    if (item.FlagAtendido == null)
                    {
                        item.FlagAtendido = "";
                    }
                    if (item.FlagAtendido == "X")
                    {
                        status = "ACTIVO";
                    }
                    else
                    {
                        status = "INACTIVO";
                        
                    }
                   


                    MtrDirecciones direccionClienteObj = await _unitOfWork.MtrDireccionesRepository.GetById((decimal)item.IdDireccion);
                  
                    
                    var direcciones = await GetDireccionestDtoByCodigo(item.Codigo);
                    if (direcciones.Count() > 0)
                    {
                        foreach (var itemDireccion in direcciones)
                        {
                            MtrClienteDireccionDto resultItem = new MtrClienteDireccionDto();
                            resultItem.Id = (decimal)itemDireccion.Id;
                            resultItem.IdDireccionCliente = (decimal)item.IdDireccion;
                            if (direccionClienteObj != null)
                            {
                                MtrDireccionesDto direccionClienteObjDto = _mapper.Map<MtrDireccionesDto>(direccionClienteObj);
                                resultItem.DireccionClienteObj = direccionClienteObjDto;

                            }
                            MtrDirecciones direccionObj = await _unitOfWork.MtrDireccionesRepository.GetById((decimal)itemDireccion.Id);
                            if (direccionObj != null)
                            {
                                MtrDireccionesDto direccionObjDto = _mapper.Map<MtrDireccionesDto>(direccionObj);
                                resultItem.DireccionObj = direccionObjDto;

                            }
                            
                            resultItem.Codigo = item.Codigo.Trim();
                            resultItem.NombreCliente = item.Nombre.Trim();
                            resultItem.Status = status;
                            if (itemDireccion.Estado == null) itemDireccion.Estado = "";
                            if (itemDireccion.Municipio == null) itemDireccion.Municipio = "";
                            resultItem.Estado = itemDireccion.Estado.Trim();
                            resultItem.Municipio = itemDireccion.Municipio.Trim();
                            resultItem.NombreEstado = "";
                            resultItem.NombreMunicipio = "";
                            if (itemDireccion.Direccion == null) itemDireccion.Direccion = "";
                            if (itemDireccion.Direccion1 == null) itemDireccion.Direccion1 = "";
                            if (itemDireccion.Direccion2 == null) itemDireccion.Direccion2 = "";
                            resultItem.Direccion = itemDireccion.Direccion.Trim();
                            resultItem.Direccion1 = itemDireccion.Direccion1.Trim();
                            resultItem.Direccion1 = itemDireccion.Direccion1.Trim();  
                            resultItem.RifCliente = item.NoRegTribut;
                            resultItem.RifDireccion = itemDireccion.Rif;
                            if (item.Segmento.Trim()== "" || item.Segmento == null)
                            {
                                item.Segmento = "0";
                                
                            }
                            resultItem.Sector = item.Segmento.Trim();
                            resultItem.DescripcionSector = "";
                            resultItem.PuntoReferencia = "";
                            var wsmy264 = await _unitOfWork.Wsmy264Repository.GetById(itemDireccion.Id);
                            if (wsmy264 != null)
                            {
                                resultItem.PuntoReferencia = wsmy264.PuntoReferencia;
                            }
                          
                            decimal sectorDecimal = decimal.Parse(resultItem.Sector);
                            var sector = await _unitOfWork.Wsmy065Repository.GetSectorBySector(sectorDecimal);
                            if (sector != null)
                            {
                                if (sector.FlagInactiva == false)
                                {
                                    resultItem.DescripcionSector = sector.NombreSector;
                                }
                                else
                                {
                                    resultItem.Sector = "";
                                    resultItem.DescripcionSector = "";
                                }
                               
                            }
                            
                            resultItem.Ramo = item.SubSegmentoa.Trim();
                            resultItem.DescripcionRamo = "";
                            if (resultItem.Ramo == "" || resultItem.Ramo == null)
                            {
                                resultItem.Ramo = "0";
                                
                            }
                            decimal ramoDecimal = decimal.Parse(resultItem.Ramo);
                            var ramo = await _unitOfWork.Wsmy065Repository.GetByRamo(ramoDecimal);
                            if (ramo != null)
                            {
                                if (ramo.FlagInactiva == false)
                                {
                                    resultItem.DescripcionRamo = ramo.NombreRamo;
                                }
                                else
                                {
                                    resultItem.Ramo = "";
                                    resultItem.DescripcionRamo = "";
                                    
                                }
                                
                            }

                            resultItem.IdTipoNegocio = item.TipoNegocio.Trim();
                            if (resultItem.IdTipoNegocio ==  "1")
                            {
                                resultItem.DescripcionTipoNegocio = "PRIVADO";
                            }else{
                                resultItem.DescripcionTipoNegocio = "GOBIERNO";
                            }
                            
                            var municipio =  await _unitOfWork.Winy243Repository.GetByEstadoMunicipio(resultItem.Estado,resultItem.Municipio );
                            if (municipio != null)
                            {
                                resultItem.DescripcionMunicipio =municipio.CapitalMcpo + " " + municipio.DescMunicipio;
                                resultItem.DescripcionEstado = municipio.NombreEstado.Trim();
                            }
                          
                            resultItem.SectorObj = sectorObj;
                            result.Add(resultItem);
                        }
                      
                    } else
                    {
                        MtrClienteDireccionDto resultItem = new MtrClienteDireccionDto();
                        resultItem.Id = (decimal)item.IdDireccion;
                        resultItem.IdDireccionCliente = (decimal)item.IdDireccion;
                        resultItem.Codigo = item.Codigo;
                        resultItem.NombreCliente = item.Nombre;
                        resultItem.Status = status;
                        resultItem.Estado = item.Estado;
                        resultItem.Municipio = item.Municipio;
                        resultItem.NombreEstado = "";
                        resultItem.NombreMunicipio = "";
                        var direccionCliente = direcciones.Where(x => x.Id == item.IdDireccion).FirstOrDefault();
                        if (direccionCliente != null)
                        {
                            if (direccionCliente.Direccion == null) direccionCliente.Direccion = "";
                            if (direccionCliente.Direccion1 == null) direccionCliente.Direccion1 = "";
                            if (direccionCliente.Direccion2 == null) direccionCliente.Direccion2 = "";

                            resultItem.Direccion = direccionCliente.Direccion;
                            resultItem.Direccion1 = direccionCliente.Direccion1;
                            resultItem.Direccion1 = direccionCliente.Direccion1;
                        }
                      
                        resultItem.RifCliente = item.NoRegTribut;
                        resultItem.RifDireccion = item.NoRegTribut;
                        resultItem.SectorObj = sectorObj;
                        
                        resultItem.PuntoReferencia = "";
                        var wsmy264 = await _unitOfWork.Wsmy264Repository.GetById( resultItem.IdDireccionCliente);
                        if (wsmy264 != null)
                        {
                            resultItem.PuntoReferencia = wsmy264.PuntoReferencia;
                        }
                        result.Add(resultItem);
                    }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        var msg = cliente;
                        throw;
                    }

                    
                    

                }
            }


            return result;

        }
        

        public async Task<ApiResponse<OdooClienteTipoSectorRamo>> UpdateTipoSectorRamoPorCliente(OdooClienteTipoSectorRamo dto)
        {


            OdooClienteTipoSectorRamo resultDto = new OdooClienteTipoSectorRamo();

            Metadata metadata = new Metadata
            {
                IsValid = true,
                Message = ""

            };

            ApiResponse<OdooClienteTipoSectorRamo> response = new ApiResponse<OdooClienteTipoSectorRamo>(resultDto);

            if (dto.Tipo < 1)
            {
                metadata.IsValid = false;
                metadata.Message = "Tipo de Negocio Invalido (1 o 2)";
                response.Data = null;
                response.Meta = metadata;
                return response;
            }

            if (dto.Tipo > 2)
            {
                metadata.IsValid = false;
                metadata.Message = "Tipo de Negocio Invalido(1 o 2)";
                response.Data = null;
                response.Meta = metadata;
                return response;
            }

            if (dto.DescripcionSector.IsNullOrEmpty())
            {
                metadata.IsValid = false;
                metadata.Message = "Descripcion de Sector Ivalida";
                response.Data = null;
                response.Meta = metadata;
                return response;
            }
            if (dto.DescripcionRamo.IsNullOrEmpty())
            {
                metadata.IsValid = false;
                metadata.Message = "Descripcion de Ramo Ivalida";
                response.Data = null;
                response.Meta = metadata;
                return response;
            }

            var sector = await _unitOfWork.Wsmy065Repository.GetSectorBySector(dto.Sector);
            var ramo = await _unitOfWork.Wsmy065Repository.GetByRamo(dto.Ramo);

            if (sector == null && dto.Sector > 0 && !dto.DescripcionSector.IsNullOrEmpty())
            {

                Wsmy064 wsmy064 = new Wsmy064();
                wsmy064.Sector = dto.Sector;
                wsmy064.NombreSector = dto.DescripcionSector;
                wsmy064.FechaCreacion = DateTime.Now;
                wsmy064.FlagInactiva = false;
                await _unitOfWork.Wsmy065Repository.AddSector(wsmy064);

            }
            
            if (ramo == null && dto.Ramo > 0 && !dto.DescripcionRamo.IsNullOrEmpty())
            {

                Wsmy065 wsmy065 = new Wsmy065();
                wsmy065.Sector = dto.Sector;
                wsmy065.Ramo = dto.Ramo;
                wsmy065.NombreRamo = dto.DescripcionRamo;
                wsmy065.FechaCreacion = DateTime.Now;
                wsmy065.FlagInactiva = false;
                await _unitOfWork.Wsmy065Repository.AddRamo(wsmy065);

            }



            var mtrCliente = GetById(dto.IdCliente);
            if (mtrCliente == null)
            {
                metadata.IsValid = false;
                metadata.Message = "Cliente no existe";
                response.Data = null;
                response.Meta = metadata;
                return response;
            }
            else
            {
                mtrCliente.TipoNegocio = dto.Tipo.ToString();
                mtrCliente.SubSegmentoa = dto.Ramo.ToString();
                mtrCliente.Segmento = dto.Sector.ToString();
                _unitOfWork.MtrClienteRepository.Update(mtrCliente);

            }

            var csmy003 = await _unitOfWork.Wsmy065Repository.GetClienteCsmy003(dto.IdCliente);
            if (csmy003 != null)
            {
                csmy003.TipoNegocio = dto.Tipo.ToString();
                csmy003.SubSegmentoa = dto.Ramo.ToString();
                csmy003.Segmento = dto.Sector.ToString();
                _unitOfWork.Wsmy065Repository.UpdateCsmy003(csmy003);

            }
            await _unitOfWork.SaveChangesAsync();

            metadata.IsValid = true;
            metadata.Message = "Cliente Actualizado Satisfactoriamente";
            response.Data = dto;
            response.Meta = metadata;
            return response;


        }

        public async Task<MtrDireccionesDto> GetDireccionestDtoById(decimal id)
        {
            var direccion = await _unitOfWork.MtrDireccionesRepository.GetById(id);

            MtrDireccionesDto result = new MtrDireccionesDto();

            if (direccion != null)
            {
                result = _mapper.Map<MtrDireccionesDto>(direccion);
            }
            else
            {
                result = null;
            }


            return result;

        }

    }
}
