using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AppService.Api.Responses;
using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.Entities;
using AppService.Core.EntitiesMaestros;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmaiService _service;


        public EmailController(IEmaiService service)
        {
            _service = service;
        }

        /// <summary>
        /// Lista as Subcategorias de Producto
        ///  
        /// 
        /// </summary>
        /// <param name="filters">Filtros a aplicar AppSubCategoryQueryFilter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<List<Email>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            
                var result = await _service.GetAll();
                
                return Ok(result);

        }
        
        
        /// <summary>
        /// Lista as Subcategorias de Producto
        ///  
        /// 
        /// </summary>
        /// <param name="filters">Filtros a aplicar AppSubCategoryQueryFilter</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<List<Email>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Send()
        {
            
            await _service.Send();
                
            return Ok();

        }
        
       



    }
}