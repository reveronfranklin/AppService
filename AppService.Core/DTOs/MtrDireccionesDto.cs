using System;
using System.Collections.Generic;
using System.Text;

namespace AppService.Core.DTOs
{
    public class MtrDireccionesDto
    {
        public decimal Id { get; set; }
        public string Codigo { get; set; }
        public string Rif { get; set; }
        public string Direccion { get; set; }
        public string Direccion1 { get; set; }
        public string Direccion2 { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public decimal IdMunicipio { get; set; }
        public string NombreMunicipio { get; set; }
        public decimal PorcFlete { get; set; }

    }

    public class MtrClienteDireccionDto
    {
        public decimal Id { get; set; }
        public string Codigo { get; set; }
        public string RifCliente { get; set; }
        public string RifDireccion { get; set; }
        public string Direccion { get; set; }
        public string Direccion1 { get; set; }
        public string Direccion2 { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        
        public string DescripcionMunicipio { get; set; }
        public string DescripcionEstado { get; set; }
        
        public decimal IdDireccionCliente { get; set; }
        public string NombreCliente { get; set; }
        public string NombreEstado { get; set; }
        //si en csmy003 flag_atendido = X status es ACTIVO DE LO CONTRARIO ES INACTIVO
        public string Status { get; set; }
        public string Ramo { get; set; }
        public string Sector { get; set; }
        public string DescripcionSector { get; set; }
        public string DescripcionRamo { get; set; }
        public string NombreMunicipio { get; set; }


        public string IdTipoNegocio { get; set; }
        public string DescripcionTipoNegocio { get; set; }
        public string PuntoReferencia { get; set; }
        public string ClaseCss { get { return GetClaseCss(Id,IdDireccionCliente); } }
        public MtrSectorDto SectorObj { get; set; }
        public MtrDireccionesDto DireccionClienteObj { get; set; }
        public MtrDireccionesDto DireccionObj { get; set; }
        
        public string GetClaseCss(decimal Id, decimal IdDireccionCliente)
        {
            string result = "";
            if (Id == IdDireccionCliente)
            {
                result = "primary";
            }
            else
            {
                result = "warning";
                           
            }
            return result;
        }

    }

    public class MtrClienteDireccionUpdateDto
    {
        public decimal Id { get; set; }
        public string Codigo { get; set; }
        public decimal IdDireccionCliente { get; set; }
        public string Direccion { get; set; }
        public string Direccion1 { get; set; }
        public string Direccion2 { get; set; }
        public string Estado { get; set; }
        public string Municipio { get; set; }
        public string IdTipoNegocio { get; set; }
        public string Sector { get; set; }
        public string Ramo { get; set; }
        public string RifCliente { get; set; }
        public string RifDireccion { get; set; }
        public string PuntoReferencia { get; set; }
        
        public string Usuario { get; set; }
    }



}
