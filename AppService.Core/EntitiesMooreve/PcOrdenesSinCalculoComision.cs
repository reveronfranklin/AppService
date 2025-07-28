using AppService.Core.DTOs.Odoo.Producto.OdooProducts;

namespace AppService.Core.EntitiesMooreve;

public class PcOrdenesSinCalculoComision
{
    public decimal Id { get; set; }
    public decimal? Orden { get; set; }
    public string? SearchText { get; set; } 
}