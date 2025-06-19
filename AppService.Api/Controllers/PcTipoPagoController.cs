using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppService.Api.Responses;
using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Comisiones;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Interfaces;
using AppService.Core.Interfaces.Comisiones;
using AppService.Core.QueryFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PcTipoPagoController : ControllerBase
    {
        private readonly ITipoPagoService _service;


        public PcTipoPagoController(ITipoPagoService service )
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
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Core.DTOs.Shared.ResultDto<List<PcTipoPagoResponseDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllManuales()
        {
               var response = await _service.GetAllManuales();
               return Ok(response);
         

        }

     
      


    }
}