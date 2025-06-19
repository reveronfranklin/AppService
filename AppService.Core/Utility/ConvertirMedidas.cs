using System;
using AppService.Core.DTOs;
using AppService.Core.DTOs.Cotizaciones;

namespace AppService.Core.Utility
{
    public static class ConvertirMedidas
    {
        public static decimal TruncateDecimal(decimal d, int decimals)
        {
            if (decimals < 0)
                throw new ArgumentOutOfRangeException("decimals", "Value must be in range 0-28.");
            else if (decimals > 28)
                throw new ArgumentOutOfRangeException("decimals", "Value must be in range 0-28.");
            else if (decimals == 0)
                return Math.Truncate(d);
            else
            {
                decimal integerPart = Math.Truncate(d);
                decimal scalingFactor = d - integerPart;
                decimal multiplier = (decimal)Math.Pow(10, decimals);

                scalingFactor = Math.Truncate(scalingFactor * multiplier) / multiplier;

                return integerPart + scalingFactor;
            }
        }
         public  static string ConvertirAPulgadasFraccion1(double centimetros)
            {
                // 1 pulgada = 2.54 centímetros
                double pulgadas = centimetros / 2.54;
        
                // Convertir la parte decimal a fracción
                double parteDecimal = pulgadas - Math.Floor(pulgadas);
                string fraccion = DecimalAFraccion(parteDecimal);
        
                // Si no hay parte decimal, devolver solo la parte entera
                if (fraccion == "0")
                {
                    return $"{(int)Math.Floor(pulgadas)}";
                }
        
                return $"{(int)Math.Floor(pulgadas)} {fraccion}";
            }
        
            static string DecimalAFraccion(double decimalValue)
            {
                // Precisión para la conversión a fracción
                double precision = 0.0001;
        
                // Fracciones comunes en pulgadas
                int[] denominadores = { 2, 4, 8, 16, 32, 64 };
                int numerador = 0;
                int denominador = 1;
        
                // Encontrar la fracción más cercana
                foreach (int d in denominadores)
                {
                    double n = Math.Round(decimalValue * d);
                    double diferencia = Math.Abs(decimalValue - (n / d));
        
                    if (diferencia < precision)
                    {
                        numerador = (int)n;
                        denominador = d;
                        break;
                    }
                }
        
                // Simplificar la fracción
                int mcd = MaximoComunDivisor(numerador, denominador);
                numerador /= mcd;
                denominador /= mcd;
        
                if (numerador == 0)
                {
                    return "0";
                }
        
                return $"{numerador}/{denominador}";
            }


            public static string ValueFormated(string medidaString)
            {
                medidaString = medidaString.Replace("/", "0");
                medidaString = medidaString.Replace(" ", "0");
                medidaString = medidaString.PadRight(5, '0');
                return medidaString;
                
            }  
        public static Medidas ConvertirCmAPulgadas(double cm,bool convertir)
        {
            Medidas result = new Medidas();
            if (cm < 0)
            {
                result.Value = 0;
                result.OriginalValue = cm;
                result.ValueFormat = "";

                result.HumanRepresentation = "";
                
                
                return result;
            }

        double pulgadasDecimal = cm / 2.54;
        if (!convertir)
        {
            pulgadasDecimal = cm;
        }
        int pulgadasEnteras = (int)Math.Floor(pulgadasDecimal);
        double pulgadasFraccionDecimal = pulgadasDecimal - pulgadasEnteras;

        if (pulgadasFraccionDecimal == 0)
        {
            result.Value =pulgadasDecimal;
            result.OriginalValue = cm;
            
            result.HumanRepresentation = $"{pulgadasEnteras}";
            result.ValueFormat = ValueFormated(    result.HumanRepresentation);

            return result ;
        }

        // Buscar la fracción más cercana (hasta denominador 8 para este ejemplo)
        int mejorNumerador = 0;
        int mejorDenominador = 1;
        double menorDiferencia = 1.0;

        for (int denominador = 2; denominador <= 8; denominador++)
        {
            int numerador = (int)Math.Round(pulgadasFraccionDecimal * denominador);
            double fraccionDecimalAproximada = (double)numerador / denominador;
            double diferencia = Math.Abs(pulgadasFraccionDecimal - fraccionDecimalAproximada);

            // Simplificar la fracción
            int gcd = GreatestCommonDivisor(numerador, denominador);
            int numeradorSimplificado = numerador / gcd;
            int denominadorSimplificado = denominador / gcd;
            double fraccionDecimalSimplificada = (double)numeradorSimplificado / denominadorSimplificado;
            double diferenciaSimplificada = Math.Abs(pulgadasFraccionDecimal - fraccionDecimalSimplificada);

            if (diferenciaSimplificada < menorDiferencia)
            {
                menorDiferencia = diferenciaSimplificada;
                mejorNumerador = numeradorSimplificado;
                mejorDenominador = denominadorSimplificado;
            }
        }

        if (pulgadasEnteras > 0)
        {
            if (mejorNumerador > 0)
            {
                
                result.Value =pulgadasDecimal;
                result.OriginalValue = cm;
                result.ValueFormat = "";

                result.HumanRepresentation =  $"{pulgadasEnteras} {mejorNumerador}/{mejorDenominador}";
                result.ValueFormat = ValueFormated(result.HumanRepresentation);
                return result;
            }
            else
            {
                result.Value =pulgadasDecimal;
                result.OriginalValue = cm;
                result.ValueFormat = "";

                result.HumanRepresentation =  $"{pulgadasEnteras} ";
                result.ValueFormat = ValueFormated(    result.HumanRepresentation);
                
                return result;
            }
        }
        else
        {
            if (mejorNumerador > 0)
            {
                result.Value =pulgadasDecimal;
                result.OriginalValue = cm;
                result.ValueFormat = "";

                result.HumanRepresentation =  $"{mejorNumerador}/{mejorDenominador} ";
                result.ValueFormat = ValueFormated(    result.HumanRepresentation);
                return result;
            }
            else
            {
                result.Value =pulgadasDecimal;
                result.OriginalValue = cm;
                result.ValueFormat = "";

                result.HumanRepresentation =  $"0";
                result.ValueFormat = ValueFormated(    result.HumanRepresentation);
                return result;
            }
        }
    }
            
            private static int GreatestCommonDivisor(int a, int b)
            {
                while (b != 0)
                {
                    int temp = b;
                    b = a % b;
                    a = temp;
                }
                return a;
            }

        
            static int MaximoComunDivisor(int a, int b)
            {
                while (b != 0)
                {
                    int temp = b;
                    b = a % b;
                    a = temp;
                }
                return a;
            }
        
        
    }
}
