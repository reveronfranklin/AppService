using AppService.Core.DTOs;
using AppService.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AppService.Core.DTOs.Sis;
using AppService.Dtos.Sis;

namespace AppService.Core.Interfaces
{
    public interface ISegUsuarioRepository 
    {
        

        Task<SegUsuario> GetUser(string id);
        Task<CredentialsDto> UsuarioValido(string Usuario, string Password);
        Task<ResultLoginDto> Login(LoginDto dto);
        void UpdateToken(SegUsuario usuario);
        Task<List<OSS_USUARIO_ROL>> GetRollByCodigoUsuario(string usuario);

    }
}
