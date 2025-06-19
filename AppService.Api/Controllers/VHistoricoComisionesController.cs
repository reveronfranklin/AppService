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
    public class VHistoricoComisionesController : ControllerBase
    {
        private readonly IHistoricoComisionesService _service;


        public VHistoricoComisionesController(IHistoricoComisionesService service )
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<List<VHistoricoComsiones>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByPeriodo(PeriodoFilter filter)
        {
               var response = await _service.GetByPeriodo(filter);
                return Ok(response);
         

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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<List<ResumenComisionResponse>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetByPeriodoResumen(PeriodoFilter filter)
        {
            var response = await _service.GetByPeriodoResumen(filter);
            return Ok(response);
         

        }


    }
}