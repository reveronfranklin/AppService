using AppService.Core.DTOs.Odoo.Producto.OdooProducts;

namespace AppService.Core.EntitiesMooreve;

public class PcOrdenesSinCalculoComision
{
    public int Id { get; set; }
    public long Orden { get; set; }
    public string SearchText { get; set; }
}