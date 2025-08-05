using AppService.Core.DTOs.Odoo.Producto.OdooProducts;

namespace AppService.Core.Entities;

public class AppVentas
{
    public decimal Id { get; set; }
    public decimal? Orden { get; set; }
    public string Producto { get; set; }
    public string SearchText { get; set; }
    public int? Alo_Contable { get; set; }
    public int? Mes_Contable { get; set; }
    public string Cliente { get; set; }
    
}