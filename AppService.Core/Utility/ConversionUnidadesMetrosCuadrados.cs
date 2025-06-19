using System;

namespace AppService.Core.Utility;

public class ConversionUnidadesMetrosCuadrados
{
    // Entradas
    public double CantidadBase { get; set; }
    public double MedidaBasica { get; set; }
    public double MedidaOpuesta { get; set; }
    public double AdicionalProduccion { get; set; }
    public double AdicionalProduccionOpuesta { get; set; }
    public double MedidaOpuestaRollo { get; set; }
    public double MedidaBasicaRollo { get; set; }
    public double Area { get; set; }

    public ConversionUnidadesMetrosCuadrados(
        double adicionalProduccion,
        double adicionalProduccionOpuesta,
        double medidaBasicaRollo,
        double medidaOpuestaRollo)
    {
        AdicionalProduccion = adicionalProduccion;
        AdicionalProduccionOpuesta = adicionalProduccionOpuesta;
        MedidaBasicaRollo = medidaBasicaRollo;
        MedidaOpuestaRollo = medidaOpuestaRollo;
        CantidadBase = 0;
        MedidaBasica = 0;
        MedidaOpuesta = 0;
        Area = 0;
    }

    public double GetCantidadPorUnidad()
    {
        // cantidadBase
        double area = ((MedidaBasica + AdicionalProduccion) / 100) *
                     ((MedidaOpuesta + AdicionalProduccionOpuesta) / 100);

        Area = area;

        // [MEDIDAOPUESTAROLLO]/([MEDIDAOPUESTA]+[ADICIONALPRODUCCIONOPUESTA])
        double unidadesPorMedidaOpuestaRollo = 
            MedidaOpuestaRollo / (MedidaOpuesta + AdicionalProduccionOpuesta);

        // [MEDIDABASICAROLLO]/([MEDIDABASICA]+[ADICIONALPRODUCCION])
        double unidadesPorBasicaRollo = 
            MedidaBasicaRollo / (MedidaBasica + AdicionalProduccion);

        // [MEDIDAOPUESTAROLLO]/([MEDIDABASICA]+[ADICIONALPRODUCCION])
        double unidadesPorMedidaOpuestaRollo2 = 
            MedidaOpuestaRollo / (MedidaBasica + AdicionalProduccion);

        // [MEDIDABASICAROLLO]/([MEDIDAOPUESTA]+[ADICIONALPRODUCCIONOPUESTA])
        double unidadesPorBasicaRollo2 = 
            MedidaBasicaRollo / (MedidaOpuesta + AdicionalProduccionOpuesta);

        // [UNIDADESPORMEDIDAPUESTAROLLO]*[UNIDADESPORBASICAROLLO]
        double cantidadPorUnidad = 
            Math.Truncate(unidadesPorMedidaOpuestaRollo) * Math.Truncate(unidadesPorBasicaRollo);

        // [UNIDADESPORMEDIDAPUESTAROLLO2]*[UNIDADESPORBASICAROLLO2]
        double cantidadPorUnidad2 = 
            Math.Truncate(unidadesPorMedidaOpuestaRollo2) * Math.Truncate(unidadesPorBasicaRollo2);

        return cantidadPorUnidad > cantidadPorUnidad2 ? cantidadPorUnidad : cantidadPorUnidad2;
    }
    
}