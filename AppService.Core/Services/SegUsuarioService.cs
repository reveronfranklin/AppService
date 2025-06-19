using AppService.Core.CustomEntities;
using AppService.Core.DTOs;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AppService.Core.DTOs.Sis;
using AppService.Core.Utility;
using AppService.Dtos.Sis;

namespace AppService.Core.Services
{
    public class SegUsuarioService : ISegUsuarioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        private readonly PaginationOptions _paginationOptions;

        public SegUsuarioService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _paginationOptions = options.Value;
        }

        public async  Task<SegUsuario> GetUsuario(string id)
        {
            return await _unitOfWork.SegUsuarioRepository.GetUser(id);
        }

        public async Task<CredentialsDto> UsuarioValido(string usuario, string password)
        {
          

            return await _unitOfWork.SegUsuarioRepository.UsuarioValido(usuario, password);

        }
        
        
        public async  Task<List<RoleMenuDto>> GetMenu(string usuario)
        {

            List<RoleMenuDto> result = new List<RoleMenuDto> ();


            var roles = await _unitOfWork.SegUsuarioRepository.GetRollByCodigoUsuario(usuario);
            
      

            if (roles!=null)
            {
                foreach (var item in roles)
                {
                    RoleMenuDto resultItem = new RoleMenuDto();
                    resultItem.Role = item.DESCRIPCION;

                    var jsonValid = JsonValidator.IsValidJson(item.JSON_MENU);
                    if (jsonValid)
                    {
                        resultItem.Menu = item.JSON_MENU;
                    }
                    else
                    {
                        resultItem.Menu = "[{ \"title\": \"Nomina\",\n    \"icon\": \"mdi:file-document-outline\",}]";
                    }
                   
                    result.Add(resultItem);

                }
                
            }

            return result;

        }


        
        public async Task<ResultLoginDto> Login(LoginDto dto)
        {
            
            
            var result = await _unitOfWork.SegUsuarioRepository.Login(dto);
            if (result.Validate == true)
            {
                var usuario = await GetUsuario(dto.Login);
                result.AccessToken = GetToken(usuario);

            }
            return result;
        }

        public void UpdateToken(SegUsuario usuario)
        {
            _unitOfWork.SegUsuarioRepository.UpdateToken(usuario);
            
        }
        
        public string GetToken(SegUsuario usuario) {


            try
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,usuario.Usuario),

                    new Claim(ClaimTypes.Role,"Admin")
                };
                
                var tokenKey= "MySecretKeyjhsfkhfsd786y4ufjdsoie4hkgjhgfjdsgfklsjfokpok043985kjsdlkfjlkfjlfjsbyryblzxwri43j5hjk35h";
                
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(tokenKey));

                var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires:DateTime.Now.AddDays(30),
                    signingCredentials:cred
                );

                var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                return jwt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
          
        }
        
        public string GenerateToken(SegUsuario usuario)
        {
            //Header
            var secretKey = _configuration["Authentication:SecrectKey"];
            SymmetricSecurityKey _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            JwtHeader header = new JwtHeader(signingCredentials);

            //Claims
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.Name,usuario.Usuario),

                new Claim(ClaimTypes.Role,"admin"),

                new Claim(ClaimTypes.Email,usuario.Email),
            

            };


            //Payload

            JwtPayload payload = new JwtPayload
            (

                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.Now,
                DateTime.UtcNow.AddDays(30)

             );

            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<List<PageMenu>> GetMenuByRole(int role)
        {
           
            return await _unitOfWork.PageMenuRepository.GetMenuByRole(role);
           
        }

        public async Task<List<ChildMenu>> GetChildMenuByParent(int idMenu)
        {
            return await _unitOfWork.ChildMenuRepository.GetChildMenuByParent(idMenu);
        }

        public async Task<List<PageMenuDto>> GetMenuDtoByRole(int role)
        {

            List<PageMenuDto> result =  new List<PageMenuDto>();

            var menu = await  GetMenuByRole(role);
            if (menu!= null)
            {
                foreach (var itemMenu in menu)
                {
                    PageMenuDto resultItemMenu = new PageMenuDto();

                    resultItemMenu.Icon = itemMenu.Icon;
                    resultItemMenu.Id = itemMenu.Id;
                    resultItemMenu.Role = (int)itemMenu.Role;
                    resultItemMenu.Title = itemMenu.Title;
                    resultItemMenu.Url = itemMenu.Url;

                    List<Children> resultChild = new List<Children>();
                    var child = await GetChildMenuByParent(itemMenu.Id);

                    if (child!=null)
                    {
                    
                        foreach (var itemChild in child)
                        {
                            Children itemChildDto = new Children();
                            itemChildDto.Id = itemChild.Id;
                            itemChildDto.Title = itemChild.Title;
                            itemChildDto.Url = itemChild.Url;
                            itemChildDto.PageMenuId = (int)itemChild.PageMenuId;
                            itemChildDto.Icon = itemChild.Icon;
                           
                            resultChild.Add(itemChildDto);
                        }
                        resultItemMenu.Children = resultChild;
                    }

                    result.Add(resultItemMenu);
                }
               
            }

            return result;

        }



    }
}
