using AppService.Api.Responses;
using AppService.Core.DTOs;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AppService.Core.DTOs.AppCalculadora;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.EntitiesMooreve;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppCalculadoraController : ControllerBase
    {
        private readonly IAppCalculadoraService _service;


        public AppCalculadoraController(IAppCalculadoraService service)
        {
            _service = service;
        }




        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetByVendedor(AppCalculadoraFilterDto filter)
        {
            
            var response = await _service.GetByVendedor(filter);
            return Ok(response);
            
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Update(AppCalculadoraUpdateDto dto)
        {
            
            var response = await _service.Update(dto);
            return Ok(response);
            
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(AppCalculadoraUpdateDto dto)
        {
            
            var response = await _service.Create(dto);
            return Ok(response);
            
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Delete(AppCalculadoraDeleteDto dto)
        {
            
            var response = await _service.Delete(dto);
            return Ok(response);
            
        }
        

    }
}