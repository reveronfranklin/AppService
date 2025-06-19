using System;
using AppService.Core.Dtos;

namespace AppService.Core.DTOs.Cotizaciones;

public class TasaConsolidadasResponseDto
{
    public decimal Id { get; set; }
    public DateTime Fecha { get; set; }
    public string FechaString { get; set; }
    public FechaDto FechaObj { get; set; }
    public Decimal TasaBcv { get; set; }
    public Decimal TasaParalelo { get; set; }
    public Decimal TasaPromedio { get; set; }
    public string SearchText { get; set; }
}