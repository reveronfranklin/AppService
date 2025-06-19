using AppService.Api.Responses;
using AppService.Core.DTOs;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AppService.Core.DTOs.Aprobaciones;
using AppService.Core.EntitiesMooreve;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CotizacionesPorAprobarController : ControllerBase
    {
        private readonly IVCotizacionesPorAprobarService _service;

        public CotizacionesPorAprobarController(IVCotizacionesPorAprobarService service)
        {
            _service = service;
        }



        /// <summary>
        /// Retorna los datos de AppConfigAppGetDto
        /// Filtros a aplicar AppConfigAppQueryFilter, 
        /// se realizara el filtro de los datos
        /// </summary>
        /// <param name="filters">Filtros a aplicar AppConfigAppQueryFilter, se realizara el filtro de los datos</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<V_CotizacionesPorAprobar>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllBySearchText(AppGeneralQuotesQueryFilter filters)
        {

            filters.UsuarioConectado = filters.Usuario;
            if(filters.EstatusPlanta==null) filters.EstatusPlanta = "";
            if (filters.EstatusPlanta == "")
            {
                var response = await _service.GetAllBySearchTextAll(filters);
                return Ok(response);
            }
            else
            {
                var response = await _service.GetAllBySearchText(filters);
                return Ok(response);
            }
          



        }
        
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<V_CotizacionesPorAprobar>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateCotizacion(ActualizarCotizacion dto)
        {
            var response = await _service.UpdateCotizacion(dto);
            return Ok(response);
            
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<V_CotizacionesPorAprobar>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PasePlanta(ActualizarCotizacion dto)
        {
            var response = await _service.PasePlanta(dto);
            return Ok(response);
            

        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(List<V_CotizacionesPorAprobar>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CerrarExcepcion(ActualizarCotizacion dto)
        {
            var response = await _service.CerrarExcepcion(dto);
            return Ok(response);
            

        }

        

     



    }
}