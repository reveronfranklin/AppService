using System;
using System.Threading.Tasks;
using AppService.Core.Entities;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using AppService.Core.Interfaces;
using System.IO;
using iText.Layout;
using iText.IO.Image;
using iText.Layout.Element;
using iText.Kernel.Pdf.Xobject;
using iText.Layout.Properties;
using AppService.Core.DTOs.DocumentosFiscales;
using AppService.Core.DTOs;
using AppService.Core.EntitiesFacturacion;
using System.Collections.Generic;
using System.Linq;

namespace AppService.Core.Services
{
    public class AppDocumentosFiscalesService : IAppDocumentosFiscalesService
    {
        private readonly IUnitOfWork _unitOfWork;
        //DESARROLLO
        private readonly string _env = "development";
        //production
        //private readonly string _env = "producction";

        public AppDocumentosFiscalesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

        }

        public string ReadTextPdf(string src)
        {

            var pdfDocument = new PdfDocument(new PdfReader(src));
            var strategy = new LocationTextExtractionStrategy();
            string text = string.Empty;
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); ++i)
            {
                var page = pdfDocument.GetPage(i);
                // text = PdfTextExtractor.GetTextFromPage(page);

                PdfDictionary infoDictionary = pdfDocument.GetTrailer().GetAsDictionary(PdfName.Info);
                foreach (PdfName key in infoDictionary.KeySet())
                {
                    text = $"{text}{key}: {infoDictionary.GetAsString(key)}";
                }




            }
            return text;

        }

        public string[] GetFicheros( string ruta )
        {

            AppRutasDocumentosFiscalesDto rutas = new AppRutasDocumentosFiscalesDto(_env);

            //string ruta = _paginationOptions.FolderFiscales ;
           // string ruta = rutas.FolderFiscales;
            string directorio = ruta;
            //string directorio = @"/Users/freveron/Documents/Moore/Facturacion/enProceso/";
            string[] ficheros = Directory.GetFiles(directorio);
            string[] sorted = ficheros.OrderByDescending(o => o).ToArray();
            return sorted;
        }

        public void Convert(string srcFile, string outFileName, string control, string anulado = "")
        {
            try
            {
                if (anulado == null) anulado = string.Empty;

                PdfDocument pdf = new PdfDocument(new PdfReader(srcFile), new PdfWriter(outFileName));

                Document doc = new Document(pdf);




                //string imageFile = "/Users/freveron/Documents/Moore/Facturacion/image/encabezado.jpg";
                //string imageFilePie = "/Users/freveron/Documents/Moore/Facturacion/image/PiePagina.png";
                AppRutasDocumentosFiscalesDto rutas = new AppRutasDocumentosFiscalesDto(_env);

                string imageFile = rutas.FolderFiscalesImagesEncabezado;
                string imageFilePie = rutas.FolderFiscalesImagesPie;
                PdfImageXObject xObject = new PdfImageXObject(ImageDataFactory.Create(imageFile));
                Image image = new Image(xObject, 100).SetHorizontalAlignment(HorizontalAlignment.LEFT);
                doc.SetTopMargin(0);
                doc.SetBottomMargin(0);
                //doc.SetRightMargin(0);
                var witch = image.GetImageScaledWidth();
                var height = image.GetImageScaledHeight();
                image.ScaleToFit(13500, height * 50);
                doc.Add(image);


                PdfImageXObject xObjectPie = new PdfImageXObject(ImageDataFactory.Create(imageFilePie));
                Image imagePie = new Image(xObjectPie, 100).SetHorizontalAlignment(HorizontalAlignment.LEFT);

                //doc.SetRightMargin(0);

                imagePie.ScaleToFit(13500, height * 50);
                imagePie.SetFixedPosition(5, 15);
                doc.Add(imagePie);

                Paragraph pControl = new Paragraph("0" + control).SetFontColor(iText.Kernel.Colors.ColorConstants.RED).SetFontSize(14);
                pControl.SetFixedPosition(480, 17, 200);
                doc.Add(pControl);
                if (anulado.Length > 0)
                {
                    Paragraph pAnulado = new Paragraph("DOCUMENTO ANULADO").SetFontColor(iText.Kernel.Colors.ColorConstants.RED).SetFontSize(14);
                    pAnulado.SetFixedPosition(180, 150, 100);
                    doc.Add(pAnulado);
                }




                doc.Close();
                pdf.Close();
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
            }


        }

        public async Task<string> ReadLineByLineTextPdf(string src)
        {

            string text1 = "";
            try
            {
                
                string outFileName = @"";


                AppRutasDocumentosFiscalesDto rutas = new AppRutasDocumentosFiscalesDto(_env);
                var destino = rutas.FolderFiscalesHistorico;

                var respaldo = rutas.FolderFiscalesRespaldo;
                var originalSap = rutas.FolderFiscales;
                var ficheros = GetFicheros(originalSap);
                foreach (string file in ficheros)
                {
                    var srcFileArr = file.Split("/");
                    if (_env == "producction")
                    {
                        srcFileArr = file.Split("\\");
                    }
                    var fileName = srcFileArr[srcFileArr.Length - 1];
                    var controlArray = fileName.Split("_");
                    var controlFinalArray = controlArray[controlArray.Length - 1].Split(".");
                    var control = controlFinalArray[0];
                    outFileName = $"{destino}{fileName}";
                    if (!File.Exists($"{rutas.FolderFiscalesProceso}{fileName}"))
                    {

                        File.Copy(file, $"{rutas.FolderFiscalesProceso}{fileName}");
                     
                    }
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }

                text1 = $"{ficheros.Length} Archivos procesados";
                ficheros = GetFicheros(rutas.FolderFiscalesProceso);
                foreach (string file in ficheros)
                {

                    var srcFileArr = file.Split("/");
                    if (_env == "producction")
                    {
                        srcFileArr = file.Split("\\");
                    }
                    var fileName = srcFileArr[srcFileArr.Length - 1];
                    var controlArray = fileName.Split("_");
                    var controlFinalArray = controlArray[controlArray.Length - 1].Split(".");
                    var control = controlFinalArray[0];
                        outFileName = $"{destino}{fileName}";
                    var isValid = IsValidPdf(file);
                    if (isValid)
                    {
                        if (!File.Exists($"{respaldo}{fileName}"))
                        {

                            File.Copy(file, $"{respaldo}{fileName}");
                        }
                        
                        Console.WriteLine("Actualizando archivo: " + fileName);
                        var tipoDocumento = GetTipoDocumento(fileName);
                        var documento = GetDocumento(fileName);
                        var ordenCotizacion = await GetOrdenCotizacion(tipoDocumento, documento);
                        Convert(file, outFileName, control, ordenCotizacion.DocumentoAnulado);
                        if (File.Exists(file) && File.Exists(outFileName))
                        {
                            await UpdateFile(fileName, outFileName);
                            var adjunto = await _unitOfWork.OfdAdjuntoRepository.GetByFileName(fileName);
                            if (adjunto is not null && adjunto.IdTipoDocumento>0)
                            {
                                if (File.Exists(file))
                                {
                                    File.Delete(file);
                                }
                            }


                        }
                    }
                    else
                    {
                        if (File.Exists(file))
                        {
                            File.Delete(file);
                        }
                    }
                }



                return text1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return text1;
            }


        }

        public async Task UpdateFile(string fileName, string outFileName)
        {


            try
            {
                string result = string.Empty;
                var tipoDocumento = GetTipoDocumento(fileName);
                var documento = GetDocumento(fileName);
                var ordenCotizacion = await GetOrdenCotizacion(tipoDocumento, documento);

                //var ofdTipoDocumento = await _unitOfWork.OfdTipoDocumentoRepository.GetByFileNAme(fileName);

                OfdAdjunto ofdAdjunto = new OfdAdjunto();
                ofdAdjunto.NombreArchivo = fileName;

                // ofdAdjunto.Archivo = new byte[file.ContentLength];
                // file.InputStream.Read(ofdadjunto.Archivo, 0, file.ContentLength);


                byte[] bytes = File.ReadAllBytes(outFileName);
                ofdAdjunto.Archivo = bytes;
                ofdAdjunto.IdUsuarioCreacion = "SISTEMA";
                ofdAdjunto.FechaCreacion = DateTime.Now;
                short idTipoDocumento = 0;
                if (tipoDocumento == "Factura") idTipoDocumento = 15;
                if (tipoDocumento == "Nota_Credito") idTipoDocumento = 22;
                if (tipoDocumento == "Nota_Debito") idTipoDocumento = 23;
                if (tipoDocumento == "Entrega") idTipoDocumento = 16;
                if (idTipoDocumento <= 0)
                {
                    idTipoDocumento = 15;
                }
                ofdAdjunto.IdTipoDocumento = idTipoDocumento;
               
                var adjunto = await _unitOfWork.OfdAdjuntoRepository.GetByFileName(fileName);
                if (adjunto != null)
                {
                    await _unitOfWork.OfdAdjuntoCriterioRepository.DeleteGyIdAdjunto(adjunto.IdAdjunto);
                    await _unitOfWork.OfdAdjuntoRepository.Delete(adjunto.IdAdjunto);
                    await _unitOfWork.SaveChangesAsync();
                }
                    var created = await _unitOfWork.OfdAdjuntoRepository.Add(ofdAdjunto);
                    await _unitOfWork.SaveChangesAsync();

                    if (created.IdAdjunto > 0)
                    {
                        string valor = string.Empty;

                        short idCriterioBusquedaOrden = 5;
                        short idCriterioBusquedaCotizacion = 6;
                        short idCriterioBusquedaCliente = 7;
                        short idCriterioBusquedaRif = 8;
                        short idCriterioBusquedaFactura = 9;
                        short idCriterioBusquedaNotaEntrega = 11;
                        short idCriterioBusquedaNotaCredito = 13;
                        short idCriterioBusquedaNotaDebito = 14;

                        if (ordenCotizacion.Orden == null) ordenCotizacion.Orden = 0;
                        if (ordenCotizacion.Cotizacion == null) ordenCotizacion.Cotizacion = "";
                        if (ordenCotizacion.Cliente == null) ordenCotizacion.Cliente = "";
                        if (ordenCotizacion.Rif == null) ordenCotizacion.Rif = "";
                        
                        if (tipoDocumento == "Factura" && created.IdAdjunto > 0)
                        {
                            valor = documento.ToString();
                            await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaFactura, valor);
                        }
                        if (tipoDocumento == "Nota_Credito" && created.IdAdjunto > 0)
                        {
                            valor = documento.ToString();
                            await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaNotaCredito, valor);
                        }
                        if (tipoDocumento == "Nota_Debito" && created.IdAdjunto > 0)
                        {
                            valor = documento.ToString();
                            await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaNotaDebito, valor);
                        }
                        if (tipoDocumento == "Entrega" && created.IdAdjunto > 0)
                        {
                            valor = documento.ToString();
                            await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaNotaEntrega, valor);
                        }

                        if (ordenCotizacion.Orden > 0)
                        {
                            valor = ordenCotizacion.Orden.ToString();
                            await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaOrden, valor);


                        }
                        if (ordenCotizacion.Cotizacion.Length > 0)
                        {

                            valor = ordenCotizacion.Cotizacion.ToString();
                            await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaCotizacion, valor);
                        }
                        if (ordenCotizacion.Cliente.Length > 0)
                        {
                            valor = ordenCotizacion.Cliente.ToString();
                            await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaCliente, valor);
                        }
                        if (ordenCotizacion.Rif.Length > 0)
                        {
                            valor = ordenCotizacion.Rif.ToString();
                            await AddAdjuntoCriterio(created.IdAdjunto, idCriterioBusquedaRif, valor);
                        }

                    }


           

              


            }
            catch (Exception ex)
            {
                    var msg = ex.Message;
            }





        }

        private async Task<OfdAdjuntoCriterio> AddAdjuntoCriterio(long IdAdjunto, Int16 IdCriterioBusqueda, string Valor)
        {
            OfdAdjuntoCriterio AdjCri = new OfdAdjuntoCriterio();
            try
            {
                AdjCri.IdAdjunto = IdAdjunto;
                AdjCri.IdCriterioBusqueda = IdCriterioBusqueda;
                AdjCri.Valor = Valor;
                AdjCri.IdUsuarioCreacion = "CLIENTE";
                AdjCri.FechaCreacion = DateTime.Now;
                var created = await _unitOfWork.OfdAdjuntoCriterioRepository.Add(AdjCri);
                await _unitOfWork.SaveChangesAsync();
                return created;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        
        }

        public List<CriterioBusqueda> GetCriterioBusqueda()
        {

            List<CriterioBusqueda> lista = new List<CriterioBusqueda>()
            {
                new CriterioBusqueda() { Id = 5, Descripcion= "Orden" },
                new CriterioBusqueda() { Id = 6, Descripcion= "Cotizacion" },
                new CriterioBusqueda() { Id = 7, Descripcion= "Cliente" },
                new CriterioBusqueda() { Id = 8, Descripcion= "RIF" },
                new CriterioBusqueda() { Id = 9, Descripcion= "Factura" },
                new CriterioBusqueda() { Id = 11, Descripcion= "Remision" },
                new CriterioBusqueda() { Id = 12, Descripcion= "Nota Credito" },
                 new CriterioBusqueda() { Id = 13, Descripcion= "Nota Debito" },

            };

            return lista;

        }

        public string GetTipoDocumento(string fileName)
        {
            string result = string.Empty;

            var esFactura = fileName.Contains("Factura");
            var esNotaCredito = fileName.Contains("Nota_Cr");
            var esNotaDebito = fileName.Contains("Nota_D");
            var esEntrega = fileName.Contains("Entrega");
            if (esFactura) result = "Factura";
            if (esNotaCredito) result = "Nota_Credito";
            if (esNotaDebito) result = "Nota_Debito";
            if (esEntrega) result = "Entrega";
            return result;
        }
        public string GetNumeroDocumento(string fileName)
        {
            string result = string.Empty;

            var ultimos = fileName.PadRight(14);
            var docArray = ultimos.Split(".");
            result = docArray[0];

            return result;
        }
        public async Task<OrdenCotizacion> GetOrdenCotizacion(string tipoDocumento, long documento)
        {
            OrdenCotizacion result = new OrdenCotizacion();
            result.DocumentoAnulado = "";
            switch (tipoDocumento)
            {
                case "Factura":
                    var factura = await _unitOfWork.Ciny057Repository.GetByNumero((int)documento);
                    if (factura != null)
                    {
                        result.Orden = factura.Orden;
                        var ventas = await _unitOfWork.VentasRepository.GetByOrden(factura.Orden);
                        if (ventas != null)
                        {
                            result.Cotizacion = ventas.CotizacionCorta;
                            result.Cliente = ventas.Cliente.ToString();
                            var cliente = await _unitOfWork.MtrClienteRepository.GetByIdAsync(ventas.Cliente.ToString());
                            if (cliente != null)
                            {
                                result.Rif = cliente.NoRegTribut;
                            }
                            else
                            {
                                result.Rif = "";
                            }
                        }
                        else
                        {
                            var generalFactura = await _unitOfWork.Ciny056Repository.GetByNumero((int)documento);
                            if (factura.Cotizacion.Length >= 13)
                            {
                                result.Cotizacion = factura.Cotizacion.Substring(0, 13);
                            }
                            else
                            {
                                result.Cotizacion = "";
                            }
                            
                            result.Cliente = generalFactura.Cliente.ToString();
                            result.Orden = factura.Orden;

                            var cliente = await _unitOfWork.MtrClienteRepository.GetByIdAsync(result.Cliente);
                            if (cliente != null)
                            {
                                result.Rif = cliente.NoRegTribut;
                            }
                            else
                            {
                                result.Rif = "";
                            }
                        }

                    }
                    else
                    {
                        var facturaEncabezado = await _unitOfWork.Ciny056Repository.GetByNumero((int)documento);
                        if (facturaEncabezado != null && facturaEncabezado.Anulada == "X")
                        {
                            result.DocumentoAnulado = "DOCUMENTO ANULADO";
                            result.Orden = 0;
                            result.Cotizacion = "";
                            result.Rif = "";
                            result.Cliente = "";
                        }

                    }

                    // code block
                    break;
                case "Nota_Crédito":
                    var notaCredito = await _unitOfWork.Ciny057Repository.GetNotaCreditoByNumero((int)documento);
                    if (notaCredito != null)
                    {
                        var facturaNota = await _unitOfWork.Ciny057Repository.GetByNumero(notaCredito.Factura);
                        if (facturaNota != null)
                        {
                            result.Orden = facturaNota.Orden;
                            var ventas = await _unitOfWork.VentasRepository.GetByOrden(facturaNota.Orden);
                            if (ventas != null)
                            {
                                result.Cotizacion = ventas.CotizacionCorta;
                                result.Cliente = ventas.Cliente.ToString();
                                var cliente = await _unitOfWork.MtrClienteRepository.GetByIdAsync(ventas.Cliente.ToString());
                                if (cliente != null)
                                {
                                    result.Rif = cliente.NoRegTribut;
                                }
                                else
                                {
                                    result.Rif = "";
                                }
                            }

                        }
                    }
                    
                    // code block
                    break;
                case "Nota_Débito":

                    // code block
                    break;
                case "Entrega":
                    Ciny208 notaEntrega = new Ciny208();
                    var entregaFiscal = await _unitOfWork.SapEntregasFiscalesRepository.GetByDocumento(documento.ToString());
                    if (entregaFiscal != null)
                    {
                        string documentoString = entregaFiscal.EfVbeln;
                        var remision = documentoString.Substring(2);

                        notaEntrega = await _unitOfWork.Ciny057Repository.GetNotaEntregaByRemision(Int32.Parse(remision));
                    }
                    else
                    {
                        notaEntrega = await _unitOfWork.Ciny057Repository.GetNotaEntregaByDocumentoSap(documento);
                    }


                    if (notaEntrega != null)
                    {

                        result.Orden = notaEntrega.Orden;
                        var ventas = await _unitOfWork.VentasRepository.GetByOrden(notaEntrega.Orden);
                        if (ventas != null)
                        {
                            result.Cotizacion = ventas.CotizacionCorta;
                            result.Cliente = ventas.Cliente.ToString();
                            var cliente = await _unitOfWork.MtrClienteRepository.GetByIdAsync(ventas.Cliente.ToString());
                            if (cliente != null)
                            {
                                result.Rif = cliente.NoRegTribut;
                            }
                            else
                            {
                                result.Rif = "";
                            }
                        }


                    }
                    // code block
                    break;
                default:
                    // code block
                    break;
            }




            return result;

        }
        public long GetDocumento(string fileName)
        {
            long result = 0;

            var tipoDocumento = GetTipoDocumento(fileName);
            var numeroDocumento = GetNumeroDocumento(fileName);

            switch (tipoDocumento)
            {
                case "Factura":
                    var arrayDocumento = numeroDocumento.Split("_");
                    var facturaString = arrayDocumento[1].Substring(4);
                    result = Int32.Parse(facturaString);
                    // code block
                    break;
                case "Nota_Credito":
                    var arrayNotaCredito = numeroDocumento.Split("_");
                    var notaCreditoString = arrayNotaCredito[2].Substring(4);
                    result = Int32.Parse(notaCreditoString);
                    // code block
                    break;
                case "Nota_Debito":
                    var arrayNotaDebito = numeroDocumento.Split("_");
                    var notaDebitoString = arrayNotaDebito[2].Substring(4);
                    result = Int32.Parse(notaDebitoString);
                    // code block
                    break;
                case "Entrega":
                    var arrayentrega = numeroDocumento.Split("_");
                    var entregaString = arrayentrega[1];
                    result = Int64.Parse(entregaString);
                    // code block
                    break;
                default:
                    // code block
                    break;
            }


            return result;
        }

        public bool IsValidPdf(string fileName)
        {
            try
            {
                new PdfReader(fileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }




    }
}

