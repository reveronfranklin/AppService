using AppService.Core.DTOs;
using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppService.Core.DTOs.Sis;
using AppService.Dtos.Sis;

namespace AppService.Infrastructure.Repositories
{
    public class SegUsuarioRepository : ISegUsuarioRepository
    {
        private readonly RRDContext _context;

        public SegUsuarioRepository(RRDContext context) 
        {
            _context = context;
        }

        public async Task<SegUsuario> GetUser(string id)
        {
            var result = await _context.SegUsuario.Where(x => x.Usuario == id ).FirstOrDefaultAsync();
            return result;
        }

        
        public async Task<List<OSS_USUARIO_ROL>> GetRollByCodigoUsuario(string usuario)
        {
            try
            {
                var result = await _context.OSS_USUARIO_ROL.DefaultIfEmpty().Where(x => x.USUARIO == usuario).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
                return null;
            }


        }
        
      
        public async Task<ResultLoginDto> Login(LoginDto dto)
        {

            ResultLoginDto resultLogin = new ResultLoginDto();
            try
            {

                var usuario = await GetUser(dto.Login);
                if (usuario == null)
                {
                    resultLogin.Message = "Usuario o Clave invalidos";
                    resultLogin.AccessToken = "";
                    resultLogin.Name = "";
                    UserData userData = new UserData();
                    userData.Id = 0;
                    userData.username = "";
                    userData.FullName = "";
                    userData.Role = "";
                    userData.Roles = null;
                    userData.Email = "";
                    resultLogin.UserData = userData;
                    return resultLogin;
                }
                
                var resultDiario = await UsuarioValido(dto.Login, dto.Password);
                if (resultDiario.Validate==false)
                {
                    resultLogin.Message = "Usuario o Clave invalidos";
                    resultLogin.AccessToken = "";
                    resultLogin.Name = "";
                    UserData userData = new UserData();
                    userData.Id = 0;
                    userData.username = "";
                    userData.FullName = "";
                    userData.Role = "";
                    userData.Roles = null;
                    userData.Email = "";
                    resultLogin.UserData = userData;
                    return resultLogin;
                }
                else
                {
                
                    resultLogin.Message = "";
                    resultLogin.Validate = true;
                    resultLogin.AccessToken = resultDiario.Token;
                    resultLogin.Name = usuario.NombreUsuario;
                    UserData userData = new UserData();
                    userData.Id = usuario.IdUsuario;
                    userData.username = usuario.NombreUsuario;
                    userData.FullName = usuario.NombreUsuario;
                    userData.Role = "admin";
                    userData.Roles = null;
                    userData.Email = usuario.Email;
                    resultLogin.UserData = userData;
                    return resultLogin;
                }

               
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                resultLogin.Message = ex.Message;
                resultLogin.AccessToken = "";
                resultLogin.Name = "";
                UserData userData = new UserData();
                userData.Id = 0;
                userData.username = "";
                userData.FullName = "";
                userData.Role = "";
                userData.Roles = null;
                userData.Email = "";
                resultLogin.UserData = userData;
                return resultLogin;
            }



         


        }



        public void UpdateToken(SegUsuario usuario)
        {
            try
            {
                FormattableString xquery = $"UPDATE SegUsuario SET REFRESHTOKEN='{usuario.REFRESHTOKEN}',TOKENCREATED={usuario.TOKENCREATED},TOKENEXPIRES={usuario.TOKENEXPIRES} WHERE usuario={usuario.Usuario};";
                var result = _context.Database.ExecuteSqlInterpolated(xquery);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
             
            }
            
         
        }
        
        public async Task<CredentialsDto> UsuarioValido(string Usuario, string Password)
        {

            
            CredentialsDto credentialsDto = new CredentialsDto();
            credentialsDto.Validate = false;
            //var usu = await _context.SegUsuario.Where(x => x.Usuario == Usuario ).FirstOrDefaultAsync();

                var BuscarUsuario = (from u in _context.SegUsuario
                                     join ur in _context.SegUsuarioRol on u.IdUsuario equals ur.IdUsuario
                                     join r in _context.SegRol on ur.IdRol equals r.IdRol
                                     where u.Usuario == Usuario & r.IdPrograma == 141
                                     select new { Clave = u.Clave, IdRol = r.IdRol, NombreUsuario = u.NombreUsuario }).FirstOrDefault();

                if (BuscarUsuario != null)
                {

                    if (BuscarUsuario.Clave == ConvertSha1(Password))
                    {

                        credentialsDto.Validate = true;
                        credentialsDto.User = Usuario;
                        credentialsDto.Role = BuscarUsuario.IdRol.ToString();



                    }
                    
                }

            

                return credentialsDto;

        }

        public string ConvertSha1(string str)
        {
            System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1Managed.Create();
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] stream = null;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            stream = sha1.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i <= (stream.Length - 1); i++)
            {
                sb.AppendFormat("{0:x2}", stream[i]);
            }
            return sb.ToString();
        }

      

       
    }
}
