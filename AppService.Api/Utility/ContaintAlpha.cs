using System;
using System.Linq;

namespace AppService.Api.Utility;

public static class ContaintAlpha
{
    public static bool ContieneLetras(string value)
    {
        
     
 
   
        bool isAlpha = value.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'));
        return isAlpha;
    }
}