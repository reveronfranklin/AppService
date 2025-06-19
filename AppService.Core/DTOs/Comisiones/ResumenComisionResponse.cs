using System;
using System.Collections.Generic;

namespace AppService.Core.DTOs.Comisiones;

public class ResumenComisionResponse
{
    
    public int Id { get; set; }
    public string Concepto { get; set; }
    public string TipoPago { get; set; }
    public decimal Comision { get; set; }
    public decimal BaseComision { get; set; }
    public string Porcentaje
    {
        get
        {
            return $"{ Math.Round((Comision/BaseComision)*100,2)}";
        }
    }
    public string SearchText
    {
        get
        {
            return $"{Concepto}-{TipoPago}";
        }
    }
   
}


