using AppService.Api.Responses;
using AppService.Core.DTOs;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class AppCostosEmbarqueController : ControllerBase
    {
        private readonly IAppCostosEmbarqueService _service;
        private readonly IUnitOfWork _unitOfWork;

        public AppCostosEmbarqueController(IAppCostosEmbarqueService service, IUnitOfWork unitOfWork)
        {
            _service = service;
            _unitOfWork = unitOfWork;
        }





        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<AppConfigAppGetDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CrearCostosDelMes()
        {

            var winy244 = await _unitOfWork.Winy244Repository.GetUltimoPeriodo();
            await _service.CrearCostosDelMes((int)winy244.AñoContable,(int) winy244.MesContable,"FA");
            await _service.CrearCostosDelMes((int)winy244.AñoContable, (int)winy244.MesContable,"NC");
            return Ok();



        }



     



    }
}