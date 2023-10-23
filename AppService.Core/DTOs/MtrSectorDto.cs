using System.Collections.Generic;
using System.ComponentModel;

namespace AppService.Core.DTOs;

public class MtrSectorDto
{
    public decimal Sector { get; set; }
    public string DescripcionSector { get; set; }

    public List<MtrRamoDto> Ramo {
        get;
        set;
    }

}