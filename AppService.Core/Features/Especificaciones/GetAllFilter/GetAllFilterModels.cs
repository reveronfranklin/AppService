using System.Collections.Generic;

namespace AppService.Core.Features.Especificaciones.GetAllFilter
{
    public class TipoPapelGramajeKey
    {
        public string TipoPapel { get; set; }
        public string Gramaje { get; set; }
    }

    public class MedidasLookup
    {
        public string MedidaBasica { get; set; }
        public string MedidaOpuesta { get; set; }
    }

    public class Wpry251SelectionLookup
    {
        public Wpry251SelectionLookup()
        {
            Keys = new HashSet<string>();
        }

        public HashSet<string> Keys { get; }

        public static string BuildKey(int idParte, string idVariable, string valor)
        {
            return $"{idParte}|{(idVariable ?? string.Empty).Trim()}|{(valor ?? string.Empty).Trim()}";
        }
    }
}
