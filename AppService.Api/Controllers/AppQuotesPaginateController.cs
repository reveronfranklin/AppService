using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppService.Api.Responses;
using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.DTOs.Ventas;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.Interfaces.Comisiones;
using AppService.Core.Interfaces.Cotizaciones;
using AppService.Core.Interfaces.Ventas;
using AppService.Core.QueryFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppQuotesPaginateController : ControllerBase
    {
        private readonly IAppQuotesPaginateService _service;


        public AppQuotesPaginateController(IAppQuotesPaginateService service )
        {
            _service = service;
        }


        /// <summary>
        /// Retorna los datos de AppUnitsGetDto
        /// Filtros a aplicar AppGeneralQuotesQueryFilter, 
        /// se realizara el filtro de los datos
        /// </summary>
        /// <param name="filters">Filtros a aplicar AppUnitsQueryFilter, se realizara el filtro de los datos</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Core.DTOs.Comisiones.ResultDto<List<AppVentas>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPaginate(AppQuotesPaginateFilter filter)
        {
               var response = await _service.GetPaginate(filter);
               return Ok(response);
         

        }

      



    }
}