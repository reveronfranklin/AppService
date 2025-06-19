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
    public class PagosManualesController : ControllerBase
    {
        private readonly IPagosManualesService _service;


        public PagosManualesController(IPagosManualesService service )
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Core.DTOs.Comisiones.ResultDto<List<PagosMAnualesResponseDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetPaginate(PagosManualesFilter filter)
        {
               var response = await _service.GetPaginate(filter);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Core.DTOs.Comisiones.ResultDto<Wsmy685>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Update(PagosManualesUpdateDto dto)
        {
            var response = await _service.Update(dto);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Core.DTOs.Comisiones.ResultDto<Wsmy685>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create(PagosManualesUpdateDto dto)
        {
            var response = await _service.Create(dto);
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Core.DTOs.Comisiones.ResultDto<Wsmy685>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Delete(PagosManualesDeleteDto dto)
        {
            var response = await _service.Delete(dto);
            return Ok(response);
         

        }



    }
}