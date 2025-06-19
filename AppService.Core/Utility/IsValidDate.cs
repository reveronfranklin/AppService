using System;

namespace AppService.Core.Utility;

public static class IsValidDateGeneral
{
    public static bool IsValidDate(int year, int month, int day)
    {
        // Validar rango del año (puedes ajustar según tus necesidades)
        if (year < 1 || year > 9999)
            return false;

        // Validar rango del mes
        if (month < 1 || month > 12)
            return false;

        // Validar rango del día
        if (day < 1 || day > DateTime.DaysInMonth(year, month))
            return false;

        return true;
    }
}