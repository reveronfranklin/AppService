using System;
using System.Linq;

namespace AppService.Core.Utility;

public static class ContaintAlpha
{
    public static bool ContieneLetras(string value)
    {

        bool result;
        result = false;
        foreach (char c in value){
            if (Char.IsLetter(c))
            {
                result = true;
                return result;
            }
            else
            {
                result = false;
            }
        }



        return result;
    }
}