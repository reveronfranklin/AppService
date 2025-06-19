namespace AppService.Core.Utility;

public class Conversion
{
    // Entradas
    public double XNumerador { get; set; }
    public double YDenominador { get; set; }
    public double CantidadBase { get; set; }

    // Resultado
    public double CantidadAlternativa { get; set; }

    public Conversion(double xNumerador, double yDenominador, double cantidadBase)
    {
        XNumerador = xNumerador;
        YDenominador = yDenominador;
        CantidadBase = cantidadBase;
        CantidadAlternativa = 0;
    }

    // Métodos
    public double GetCantidadAlternativa()
    {
        double cociente = XNumerador / YDenominador;
        CantidadAlternativa = CantidadBase * cociente;
        return CantidadAlternativa;
    }
}