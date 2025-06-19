namespace AppService.Core.DTOs.Cotizaciones;

public class TasasConsolidadasUpdateDto
{
    public int Day { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    
    public decimal TasaBcv { get; set; }
    public decimal TasaParalelo { get; set; }
    public decimal TasaPromedio { get; set; }
  
    
}