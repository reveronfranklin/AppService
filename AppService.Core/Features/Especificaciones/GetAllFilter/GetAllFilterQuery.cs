using AppService.Core.DTOs.Especificaciones;

namespace AppService.Core.Features.Especificaciones.GetAllFilter
{
    public class GetAllFilterQuery
    {
        public GetAllFilterQuery(PartesFilter filter)
        {
            Filter = filter;
        }

        public PartesFilter Filter { get; }
    }
}
