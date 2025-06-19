using AppService.Api.Responses;
using AppService.Core.DTOs;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.EntitiesMooreve;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class TasaConsolidadoController : ControllerBase
    {
        private readonly ITasaConsolidadoService _service;


        public TasaConsolidadoController(ITasaConsolidadoService service)
        {
            _service = service;
        }




        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAll()
        {
            
            var response = await _service.GetAll();
            return Ok(response);
            
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateTasas(TasasConsolidadasUpdateDto dto)
        {
            
            var response = await _service.UpdateTasas(dto);
            return Ok(response);
            
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Delete(TasasConsolidadasUpdateDto dto)
        {
            
            var response = await _service.DeleteTasas(dto);
            return Ok(response);
            
        }
        

    }
}