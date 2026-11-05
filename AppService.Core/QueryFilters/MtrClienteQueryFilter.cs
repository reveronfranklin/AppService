using System;
using System.Collections.Generic;
using System.Text;

namespace AppService.Core.QueryFilters
{
   

    public class MtrClienteQueryFilter
{
    public string Usuario { get; set; }
    public string Oficina { get; set; }
    public string Vendedor { get; set; }
    public string Codigo { get; set; }
    public string SearchText { get; set; }
    public string Rif { get; set; }
    
    // Paginación con valores por defecto
    private int _pageSize = 10;
    public int PageSize 
    { 
        get => _pageSize;
        set => _pageSize = value > 0 ? value : 10;
    }
    
    private int _pageNumber = 1;
    public int PageNumber 
    { 
        get => _pageNumber;
        set => _pageNumber = value > 0 ? value : 1;
    }
}
}
