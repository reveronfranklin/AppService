namespace AppService.Core.DTOs.Cotizaciones;

public class AppQuotesPaginateFilter
{
    public int PageSize { get; set; } 
    public int PageNumber { get; set; }
    public string SearchText { get; set; } 
    public string UsuarioConsulta { get; set; } 
}