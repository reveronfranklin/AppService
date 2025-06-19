using AppService.Api.Responses;
using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Utility;


namespace AppService.Api.Controllers
{
    //Comentario  para ver si en el git status lo refleja como modificado. 1707

    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("charppolicy")]
    public class AppAdjuntoCotizacionController : ControllerBase
    {
        private readonly ICobAdjuntosCobranzaService _cobAdjuntosCobranzaService;
        private readonly IGeneralCobranzaService _generalCobranzaService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;
        private readonly IOfdTipoDocumentoService _ofdTipoDocumentoService;
        private readonly ILogger<CobAdjuntosCobranzaController> _logger;
        private readonly IAppProductsService _appProductsService;
        private readonly IAppAdjuntosCotizacionService _appAdjuntosCotizacionService;
        private readonly PaginationOptions _paginationOptions;

        //Constructor
        public AppAdjuntoCotizacionController(ICobAdjuntosCobranzaService cobAdjuntosCobranzaService,
                                             IGeneralCobranzaService generalCobranzaService,
                                            IMapper mapper,
                                            IUriService uriService,
                                            IOptions<PaginationOptions> options,
                                            IOfdTipoDocumentoService ofdTipoDocumentoService,
                                            ILogger<CobAdjuntosCobranzaController> logger,
                                            IAppProductsService appProductsService,
                                            IAppAdjuntosCotizacionService appAdjuntosCotizacionService)
        {
            _cobAdjuntosCobranzaService = cobAdjuntosCobranzaService;
            _generalCobranzaService = generalCobranzaService;
            _mapper = mapper;
            _uriService = uriService;
            _ofdTipoDocumentoService = ofdTipoDocumentoService;
            _logger = logger;
            _appProductsService = appProductsService;
            _appAdjuntosCotizacionService = appAdjuntosCotizacionService;
            _paginationOptions = options.Value;
        }

        
     
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<AppAdjuntosCotizacionDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllAdjuntosCotizacion(AppAdjuntosCotizacionFilter filters)
        {

          
            Metadata metadata = new Metadata
            {
                IsValid = false,
                Message = "",
                TotalCount = 0
            };
            List<AppAdjuntosCotizacionDto> result = new List<AppAdjuntosCotizacionDto>();

    

           

            try
            {

                
             
                var adjuntosCotizacion =
                    await _appAdjuntosCotizacionService.GetAdjuntosCotizacion(filters);
                System.IO.Directory.CreateDirectory(_paginationOptions.FolderDocumentation + "/" + filters.Cotizacion + "/");
                
                foreach (var item in adjuntosCotizacion.Data)
                {
                    AppAdjuntosCotizacionDto resultItem = new AppAdjuntosCotizacionDto();
                    byte[] imageBytes = new byte[0];
                    
                    imageBytes = (byte[])item.Archivo;
                    
                    var ruta = _paginationOptions.FolderDocumentation + "/" + item.Cotizacion + "/";
                    
                    //
                    
                    var link = _paginationOptions.UrlGetFiles + item.Cotizacion + "/" + item.NombreArchivo;

                    //Ruta y nombre de la imagen
                    var imageFullName = ruta + item.NombreArchivo;
                    
                    //creo el fichero
                    if (!System.IO.File.Exists(imageFullName))
                    {
                        await System.IO.File.WriteAllBytesAsync(imageFullName, imageBytes);
                      
                    }
                    
                    resultItem.Cotizacion = item.Cotizacion;
                    resultItem.Renglon = (int)item.Renglon;
                    resultItem.NombreDocumento = item.NombreDocumento;
                    resultItem.NombreArchivo = item.NombreArchivo;
                    resultItem.FechaCreacion = (DateTime)item.FechaCreacion;
                    resultItem.Ruta = ruta;
                    resultItem.Link = link;
                    
                    result.Add(resultItem);
                    
                 
                }
                
                result = result.OrderByDescending(x => x.FechaCreacion).ToList();
                if (result.Count == 0)
                {
                    metadata.IsValid = false;
                    metadata.TotalCount = 0;
                    metadata.Message = "No Data";
                    metadata.success = false;
                }
                else
                {
                    metadata.IsValid = true;
                    metadata.TotalCount = result.Count;
                    metadata.success = true;
                }
                var response = new ApiResponse<List<AppAdjuntosCotizacionDto>>(result)
                {
                    Meta = metadata
                };

                
            
                return Ok(response);
                

            }
            catch (Exception e)
            {
                return Ok();
            }

        }


        public void Dispose()
        {
            // TODO release managed resources here
        }
    }

}