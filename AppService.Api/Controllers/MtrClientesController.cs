using AppService.Api.Responses;
using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Odoo.Clientes;
using AppService.Core.DTOs.Odoo.Contactos;
using AppService.Core.Entities;
using AppService.Core.EntitiesSap;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AppService.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class MtrClientesController : ControllerBase
    {
        private readonly IMtrClienteService _mtrClienteService;
        private readonly IMapper _mapper;
        private readonly IMtrContactosService _mtrContactosService;
        private readonly IMunicipioServices _municipioServices;

        public MtrClientesController(IMtrClienteService mtrClienteService,
                                        IMapper mapper,
                                        IMtrContactosService mtrContactosService,
                                        IMunicipioServices municipioServices)
        {
            _mtrClienteService = mtrClienteService;
            _mapper = mapper;
            _mtrContactosService = mtrContactosService;
            _municipioServices = municipioServices;
        }



        /// <summary>
        /// Retorna Lista de Clientes
        /// </summary>
        /// <param name="filters">Filtros a aplicar</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<MtrClienteDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetMtrClientes([FromQuery] MtrClienteQueryFilter filters)
        {


            IEnumerable<MtrCliente> clientes = await _mtrClienteService.ListClientesPorUsuario(filters);

            IEnumerable<MtrClienteDto> clientesDtos = _mapper.Map<IEnumerable<MtrClienteDto>>(clientes);



            ApiResponse<IEnumerable<MtrClienteDto>> response = new ApiResponse<IEnumerable<MtrClienteDto>>(clientesDtos);




            return Ok(response);

        }


        /// <summary>
        /// Retorna Lista de Clientes
        /// </summary>
        /// <param name="filters">Filtros a aplicar</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<MtrClienteDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> OdooActualizarClientes([FromQuery] MtrClienteQueryFilter filters)
        {


           await _mtrClienteService.OdooActualizarClientes();


            return Ok();

        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListClientesPorUsuario(MtrClienteQueryFilter filters)
        {


            IEnumerable<MtrCliente> clientes = await _mtrClienteService.ListClientesPorUsuario(filters);

            IEnumerable<MtrClienteDto> clientesDtos = _mapper.Map<IEnumerable<MtrClienteDto>>(clientes);

            foreach (var item in clientesDtos)
            {

                item.ClaseCss = "primary";
                MtrDireccionesDto direccionFacturar = await _mtrClienteService.GetDireccionestDtoById((decimal)item.IdDireccion);
                if (direccionFacturar != null)
                {
                    if (direccionFacturar.Direccion == null) direccionFacturar.Direccion = "";
                    if (direccionFacturar.Direccion1 == null) direccionFacturar.Direccion1 = "";
                    if (direccionFacturar.Direccion2 == null) direccionFacturar.Direccion2 = "";


                    item.MtrDireccionesDto = direccionFacturar;
                  

                }
            }


            var pagedclientes = PagedList<MtrClienteDto>.Create(clientesDtos, filters.PageNumber, filters.PageSize);

            ApiResponse<IEnumerable<MtrClienteDto>> response = new ApiResponse<IEnumerable<MtrClienteDto>>(clientesDtos);


            return Ok(response);


        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListSectores(MtrClienteQueryFilter filters)
        {


            var sectores  = await _mtrClienteService.ListSectores();
            
            ApiResponse<List<MtrSectorDto>> response = new ApiResponse<List<MtrSectorDto>>(sectores);
            
            return Ok(response);


        }

        
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListDireccionesClientesPorUsuario(MtrClienteQueryFilter filters)
        {
          
                
              var clientes = await _mtrClienteService.ListDireccionesPorUsuario(filters);

            clientes = clientes.Where(x=>x.Codigo!="000000").OrderBy(x => x.Codigo).
                                                                    ThenBy(x => x.ClaseCss).ToList();


            var pagedclientes = PagedList<MtrClienteDireccionDto>.Create(clientes, filters.PageNumber, filters.PageSize);

            ApiResponse<IEnumerable<MtrClienteDireccionDto>> response = new ApiResponse<IEnumerable<MtrClienteDireccionDto>>(pagedclientes);


            return Ok(response);


        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateDireccionCliente(MtrClienteDireccionUpdateDto dto)
        {

            var response = await _mtrClienteService.UpdateDireccionCliente(dto);

            return Ok(response);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateDireccionCliente(MtrClienteDireccionUpdateDto dto)
        {

            var response = await _mtrClienteService.CreateDireccionCliente(dto);

            return Ok(response);

        }

        
        
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListContactosCliente(MtrClienteQueryFilter filters)
        {


            List<MtrContactos> contactos = await _mtrContactosService.GetByIdCliente(filters);



            IEnumerable<MtrContactosDto> contactosDtos = _mapper.Map<IEnumerable<MtrContactosDto>>(contactos);

            ApiResponse<IEnumerable<MtrContactosDto>> response = new ApiResponse<IEnumerable<MtrContactosDto>>(contactosDtos);


            return Ok(response);


        }



        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListDireccionesClienteRif(MtrClienteQueryFilter filters)
        {


            List<MtrDireccionesDto> mtrDireccionesDtos = await _mtrClienteService.GetDireccionestDtoByCodigo(filters.Codigo);

            if (filters.SearchText.Length > 0)
            {
                mtrDireccionesDtos = mtrDireccionesDtos.Where(x => x.Rif.ToLower().Contains(filters.SearchText.Trim().ToLower()) || x.Direccion.ToLower().Contains(filters.SearchText.Trim().ToLower()) || x.Direccion1.ToLower().Contains(filters.SearchText.Trim().ToLower()) || x.Estado.ToLower().Contains(filters.SearchText.Trim().ToLower())).ToList();
            }

            var cliente = _mtrClienteService.GetById(filters.Codigo);
            if (cliente != null)
            {
                mtrDireccionesDtos = mtrDireccionesDtos.Where(x => x.Rif == cliente.NoRegTribut.Trim()).ToList();
                
            }

            ApiResponse<List<MtrDireccionesDto>> response = new ApiResponse<List<MtrDireccionesDto>>(mtrDireccionesDtos);


            return Ok(response);


        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListTratamientoContacto(GenericFilter filters)
        {

            List<SapTratamientoContacto> response = await _mtrContactosService.GetAllSapTratamientoContacto();

            return Ok(response);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListGetAllSapCargoContacto(GenericFilter filters)
        {

            List<SapCargoContacto> response = await _mtrContactosService.GetAllSapCargoContacto();

            return Ok(response);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListGetAllSapDepartamentoContacto(GenericFilter filters)
        {

            List<SapDepartamentoContacto> response = await _mtrContactosService.GetAllSapDepartamentoContacto();

            return Ok(response);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListGetAllSapPoderContacto(GenericFilter filters)
        {

            List<SapPoderContacto> response = await _mtrContactosService.GetAllSapPoderContacto();

            return Ok(response);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateContactoAlTables(ContactoCreateDto dto)
        {

            var response = await _mtrContactosService.CreateContactoAlTables(dto);
            return Ok(response);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateContactoAllTables(ContactoUpdateDto dto)
        {

            var response = await _mtrContactosService.UpdateContactoAllTables(dto);
            return Ok(response);

        }



        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetContactoById(ContactoQueryFilter filter)
        {

            var response = await _mtrContactosService.GetContactoById(filter);
            return Ok(response);

        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ListMunicipios(MunicipioQueryFilter filters)
        {


            var response = await _municipioServices.GetAllFilter(filters);

            return Ok(response);

        }





        //***********************************CONTACTOS ODOO****************************************


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> OdooUpdateClienteTipoSectorRamo(OdooClienteTipoSectorRamo dto)
        {

            var response = await _mtrClienteService.UpdateTipoSectorRamoPorCliente(dto);
            return Ok(response);

        }


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> OdooCreateContacto(OdooContactoCreateDto dto)
        {

            var response = await _mtrContactosService.OdooCreateContactoAlTables(dto);
            return Ok(response);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> OdooUpdateContacto(OdooContactoUpdateDto dto)
        {

            var response = await _mtrContactosService.OdooUpdateContactoAllTables(dto);
            return Ok(response);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> OdooEnviarClentes(MtrClientesFilter dto)
        {

            await _mtrContactosService.EniarClientesAOdoo(dto.Region);
            return Ok();

        }

    }
}