using System;

namespace AppService.Core.EntitiesMooreve;

public class TasaConsolidada
{
    
    public decimal Id { get; set; }
    public DateTime Fecha { get; set; }
    public Decimal TasaBcv { get; set; }
    public Decimal TasaParalelo { get; set; }
    public Decimal TasaPromedio { get; set; }
    public string SearchText { get; set; }

}