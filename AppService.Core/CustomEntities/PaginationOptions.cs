namespace AppService.Core.CustomEntities
{
    public class PaginationOptions
    {
        public int DefaultPageSize { get; set; }
        public int DefaultPageNumber { get; set; }
        public string FolderDocumentation { get; set; }
        public string FolderFiscales { get; set; }
        public string FolderFiscalesHistorico { get; set; }
        public string FolderFiscalesRespaldo { get; set; }
        public string FolderFiscalesImagesEncabezado { get; set; }
        public string FolderFiscalesImagesPie { get; set; }
        

        
        public string UrlGetFiles { get; set; }
        public string UserSap { get; set; }

        public string PasswordSap { get; set; }
        
    }
}
