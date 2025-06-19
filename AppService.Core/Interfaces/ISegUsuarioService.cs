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
    public interface ISegUsuarioService
    {
        Task<SegUsuario> GetUsuario(string id);

        Task<CredentialsDto> UsuarioValido(string usuario, string password);

 
        string GenerateToken(SegUsuario usuario);

        Task<List<PageMenuDto>> GetMenuDtoByRole(int role);
        Task<ResultLoginDto> Login(LoginDto dto);
        string GetToken(SegUsuario usuario);
        void UpdateToken(SegUsuario usuario);
        Task<List<RoleMenuDto>> GetMenu(string usuario);
    }
}
