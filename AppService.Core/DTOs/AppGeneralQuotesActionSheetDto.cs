using AppService.Core.Entities;

namespace AppService.Core.DTOs
{
    public class AppGeneralQuotesActionSheetDto
    {

        public bool Actualizar { get; set; }

        public bool EnviarAlCliente { get; set; }

        public bool EnviarAprobacionPrecio { get; set; }

        public bool GanarPerder { get; set; }

        public bool Postergar { get; set; }

        public bool Eliminar { get; set; }

        public bool Cancel { get; set; }

        public bool ExistQuotes { get; set; }

        public bool Imprimir { get; set; }

        public bool RetornarAGrabacion { get; set; }



        public AppGeneralQuotesActionSheetDto Adapter(AppGeneralQuotesActionSheet appGeneralQuotesActionSheet)
        {
                if (appGeneralQuotesActionSheet == null)
                {
                    return new AppGeneralQuotesActionSheetDto();
                }
                Actualizar = (bool)appGeneralQuotesActionSheet.Actualizar;
                EnviarAlCliente = (bool)appGeneralQuotesActionSheet.EnviarAlCliente;
                EnviarAprobacionPrecio = (bool)appGeneralQuotesActionSheet.EnviarAprobacionPrecio;
                GanarPerder = (bool)appGeneralQuotesActionSheet.GanarPerder;
                Postergar = (bool)appGeneralQuotesActionSheet.Postergar;
                Eliminar = (bool)appGeneralQuotesActionSheet.Eliminar;
                Cancel = (bool)appGeneralQuotesActionSheet.Cancel;
                ExistQuotes = (bool)appGeneralQuotesActionSheet.ExistQuotes;
                Imprimir = (bool)appGeneralQuotesActionSheet.Imprimir;
                RetornarAGrabacion = (bool)appGeneralQuotesActionSheet.RetornarAGrabacion;
                return this;
                
        }

    }
}
