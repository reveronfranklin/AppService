using AppService.Api.Responses;
using AppService.Core.DTOs;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AppService.Core.DTOs.PowerBi;
using AppService.Core.Interfaces.PowerBi;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PowerBiOrdenesController : ControllerBase
    {
        private readonly IPowerBiOrdenesService _service;


        public PowerBiOrdenesController(IPowerBiOrdenesService service)
        {
            _service = service;
        
        }



      
     

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<AppConfigAppGetDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll(PowerBiOrdenesFilter filter)
        {


            var response = await _service.GetAll(filter);
            return Ok(response);



        }
        
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<AppConfigAppGetDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllVentas()
        {


            var response = await _service.GetAllVentas();
            return Ok(response);



        }
        
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<AppConfigAppGetDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAllVentasReducido()
        {


            var response = await _service.GetAllVentasReducido();
            return Ok(response);



        }





    




    }
}