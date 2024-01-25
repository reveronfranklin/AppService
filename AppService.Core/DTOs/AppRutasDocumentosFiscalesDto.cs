using System;
namespace AppService.Core.DTOs
{
    public class AppRutasDocumentosFiscalesDto
    {
        public string FolderFiscales { get; set; } = "/Users/freveron/Documents/Moore/Facturacion/originalSap/";
        public string FolderFiscalesHistorico { get; set; } = "/Users/freveron/Documents/Moore/Facturacion/Historico/";
        
        public string FolderFiscalesProceso { get; set; } = "/Users/freveron/Documents/Moore/Facturacion/enProceso/";
        public string FolderFiscalesRespaldo { get; set; } = "/Users/freveron/Documents/Moore/Facturacion/respaldo/";
        public string FolderFiscalesImagesEncabezado { get; set; } = "/Users/freveron/Documents/Moore/Facturacion/image/encabezado.jpg";
        public string FolderFiscalesImagesPie { get; set; } = "/Users/freveron/Documents/Moore/Facturacion/image/PiePagina.png";
        public string _ambiente { get; set; } = string.Empty;
        public AppRutasDocumentosFiscalesDto(string ambiente)
        {
            _ambiente = ambiente;
            if (ambiente == "producction")
            {
                FolderFiscales = "W:\\";
                FolderFiscalesHistorico = "G:\\Facturacion\\Historico\\";
                FolderFiscalesProceso= "G:\\Facturacion\\enProceso\\";
                FolderFiscalesRespaldo = "G:\\Facturacion\\respaldo\\";
                FolderFiscalesImagesEncabezado = "G:\\Facturacion\\Image\\encabezado.jpg";
                FolderFiscalesImagesPie = "G:\\Facturacion\\Image\\PiePagina.png";
            }
        }

    }
}

