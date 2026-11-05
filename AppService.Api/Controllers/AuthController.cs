using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

// HTML to PDF

using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Security.Claims;
using System.Threading.Tasks;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Sis;
using AppService.Core.Interfaces;
using AppService.Dtos.Sis;
using Convertidor.Dtos.Sis;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SisUsuariosController : ControllerBase
    {

        private readonly ISegUsuarioService _service;

        private IHttpContextAccessor _httpContextAccessor;
 
      


        public SisUsuariosController(ISegUsuarioService service,
                                    IHttpContextAccessor httpContextAccessor
                             
                                    )
        {

            _service = service;
            _httpContextAccessor = httpContextAccessor;
 
        }


        [HttpGet]
        [Route("[action]")]
        public ActionResult<string> GetMe()
        {
            var userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name); //_service.GetMyName();
            return Ok(userName);
        }

        [HttpGet]
        [Route("[action]"),Authorize]
        public async Task<ActionResult> GetMenu()
        {
            var userName = string.Empty;
            
            var header = Request.Headers;
            userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name); //_service.GetMyName();
            var menu = await _service.GetMenu(userName);


            return Ok(menu);
        }

      
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginDto login)
        {


            ResultLoginDto result = new ResultLoginDto();
            var cedentialDto = await IsValiduser(login);
            if (cedentialDto.Validate)
            {
                var usuario = await _service.GetUsuario(login.Login);
                if (usuario != null) result.Name = usuario.NombreUsuario;

                string token = _service.GenerateToken(usuario);

                result.AccessToken = token;
                result.Validate = cedentialDto.Validate;
                var refreshToken = GenerateRefreshToken(token,login.Login);
                result.RefreshToken = refreshToken.Refresh_Token;


                UserData userData = new UserData();
                userData.Id = usuario.IdUsuario;
                userData.Email =usuario.Email;
                userData.FullName = usuario.NombreUsuario;
                userData.Role = "admin";
                userData.username = usuario.Usuario;
                result.UserData = userData;
                await SetRefreshToken(refreshToken);

                


            }
            else
            {
                return BadRequest("Usuario o clave invalida");
      
            }
            return Ok(result);

        }

        private async Task<CredentialsDto> IsValiduser(LoginDto login)
        {
            return await _service.UsuarioValido(login.Login, login.Password);
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult>  LoginBk(LoginDto dto)
        {

            try
            {
                var result = await _service.Login(dto);
                
                if (result.AccessToken.Length > 10)
                {
                    var refreshToken = GenerateRefreshToken(result.AccessToken,dto.Login);
                    refreshToken.Login = dto.Login;
                    result.RefreshToken = refreshToken.Refresh_Token;
                    await SetRefreshToken(refreshToken);
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Usuario o clave invalida");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

           
           
          
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult>  LoginMobil(LoginDto dto)
        {

            try
            {
                var result = await _service.Login(dto);
                if (result.AccessToken.Length > 10)
                {
                    var refreshToken = GenerateRefreshToken(result.AccessToken,dto.Login);
                    refreshToken.Login = dto.Login;
                    result.RefreshToken = refreshToken.Refresh_Token;
                    await SetRefreshToken(refreshToken);
                    AuthResponse authResponse = new AuthResponse();
                    authResponse.Id = result.UserData.Id.ToString();
                    authResponse.Email = result.UserData.Email;
                    authResponse.FullName = result.UserData.FullName;
                    authResponse.Roles = result.UserData.Roles;
                    authResponse.Token = result.AccessToken;
                    authResponse.RefreshToken = result.RefreshToken;
                    return Ok(authResponse);
                }
                else
                {
                    return BadRequest("Usuario o clave invalida");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

           
           
          
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ResultLoginDto>> RefreshToken(ResultRefreshTokenDto refreshTokento)
        {
            ResultLoginDto resultLogin = new ResultLoginDto();
            //3mBx+k508wTfnpi0K+txwKWz64QFdTfsd6cP6G634khKNzs11W6zSaa2ffzx4B7D2LyXppCQcgU9odwFjK2iSQ==
                var refreshToken = refreshTokento.RefreshToken;
            // Request.Cookies["X-Refresh-Token"];
            //var token = Request.Cookies["osmmasoftToken"];
                string? userName = string.Empty;
            userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name); 
            var sisUsuario = await _service.GetUsuario(userName);
            if (sisUsuario==null)
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            if (!sisUsuario.REFRESHTOKEN.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
                //var esDiferente = "";
            }
            if (sisUsuario.TOKENEXPIRES < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = _service.GetToken(sisUsuario);
            var newRefreshToken = GenerateRefreshToken(token,userName);
            SetRefreshToken(newRefreshToken);
            //ResultRefreshTokenDto result = new ResultRefreshTokenDto();
            //result.accessToken = token;
            //result.refreshToken = newRefreshToken.Refresh_Token;

            resultLogin.Message = "";
            resultLogin.RefreshToken = newRefreshToken.Refresh_Token; 
            resultLogin.AccessToken = token;
            resultLogin.Name = sisUsuario.NombreUsuario;
            UserData userData = new UserData();
            userData.Id = sisUsuario.IdUsuario;
            userData.username = sisUsuario.Usuario;
            userData.FullName = sisUsuario.NombreUsuario;
            userData.Role = "admin";
            userData.Email =sisUsuario.Email;
            resultLogin.UserData = userData;


            return Ok(resultLogin);
        }

        private RefreshToken GenerateRefreshToken(string token,string? login)
        {
            string? userName = string.Empty;
            userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
            if (userName == null) userName = login;
            var refreshToken = new RefreshToken
            {
                Refresh_Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Token = token,
                Expires = DateTime.Now.AddDays(30),
                Created = DateTime.Now,
                Login= userName
            };

            return refreshToken;
        }
        
        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult<ResultLoginDto>> CheckStatus(ResultRefreshTokenDto refreshTokento)
        {
            ResultLoginDto resultLogin = new ResultLoginDto();
            //3mBx+k508wTfnpi0K+txwKWz64QFdTfsd6cP6G634khKNzs11W6zSaa2ffzx4B7D2LyXppCQcgU9odwFjK2iSQ==
                var refreshToken = refreshTokento.RefreshToken;
            // Request.Cookies["X-Refresh-Token"];
            //var token = Request.Cookies["osmmasoftToken"];
                string? userName = string.Empty;
            userName = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name); 
            var sisUsuario = await _service.GetUsuario(userName);
            if (sisUsuario==null)
            {
                return Unauthorized("Invalid Refresh Token.");
            }
            if (!sisUsuario.REFRESHTOKEN.Equals(refreshToken))
            {
                return Unauthorized("Invalid Refresh Token.");
                //var esDiferente = "";
            }
            if (sisUsuario.TOKENEXPIRES < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = _service.GetToken(sisUsuario);
            var newRefreshToken = GenerateRefreshToken(token,userName);
            SetRefreshToken(newRefreshToken);

            resultLogin.Message = "";
            resultLogin.RefreshToken = newRefreshToken.Refresh_Token; 
            resultLogin.AccessToken = token;
            resultLogin.Name = sisUsuario.Usuario;
            UserData userData = new UserData();
            userData.Id = sisUsuario.IdUsuario;
            userData.username = sisUsuario.NombreUsuario;
            userData.FullName = sisUsuario.NombreUsuario;
            userData.Role = "admin";
            userData.Email = sisUsuario.Email;
            resultLogin.UserData = userData;

            AuthResponse authResponse = new AuthResponse();
            authResponse.Id = resultLogin.UserData.Id.ToString();
            authResponse.Email = resultLogin.UserData.Email;
            authResponse.FullName = resultLogin.UserData.FullName;
            authResponse.Roles = resultLogin.UserData.Roles;
            authResponse.Token = resultLogin.AccessToken;
            authResponse.RefreshToken = resultLogin.RefreshToken;
            

            return Ok(authResponse);
        }

      

        private async Task SetRefreshToken(RefreshToken newRefreshToken)
        {

            

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("X-Refresh-Token", newRefreshToken.Refresh_Token, cookieOptions);
            Response.Cookies.Append("X-Auth-Token", newRefreshToken.Token, cookieOptions);
            var sisUsuario = await _service.GetUsuario(newRefreshToken.Login);

            sisUsuario.REFRESHTOKEN = newRefreshToken.Refresh_Token;
            sisUsuario.TOKENCREATED = newRefreshToken.Created;
            sisUsuario.TOKENEXPIRES = newRefreshToken.Expires;
            _service.UpdateToken(sisUsuario);
        }





    }
}
