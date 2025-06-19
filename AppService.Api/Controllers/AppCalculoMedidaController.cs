using System;
using AppService.Api.Responses;
using AppService.Core.DTOs;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AppService.Core.Utility;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppCalculoMedidaController : ControllerBase
    {


        public AppCalculoMedidaController()
        {
           
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<List<AppConfigAppGetDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ConvertirCmPulgada(MedidasDto medidas)
        {
       
            var medidaBasicaConvertida = ConvertirMedidas.ConvertirCmAPulgadas(medidas.Medida,true);
            
            return Ok(medidaBasicaConvertida);



        }

     


    }
}