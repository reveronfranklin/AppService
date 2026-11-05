using System;
using System.Threading.Tasks;
using AppService.Core.Interfaces.Comisiones;
using AppService.Core.Interfaces.Cotizaciones;
using AppService.Core.Interfaces.PowerBi;
using AppService.Core.Interfaces.Ventas;

namespace AppService.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        
        
        IAppGeneralQuotesActionSheetRepository AppGeneralQuotesActionSheetRepository { get; }
        
        IEmailRepository EmailRepository { get; }
        IPcOrdenesSinCalculoComisionRepository PcOrdenesSinCalculoComisionRepository { get; }
        IPcTipoPagoOrdenNoCalcularComisionRepository PcTipoPagoOrdenNoCalcularComisionRepository { get; }
        
        
        IPowerBiOrdenesRepository PowerBiOrdenesRepository { get; }
        
        IAppCalculadoraRepository AppCalculadoraRepository { get; }
        IAppQuotesPaginateRepository AppQuotesPaginateRepository { get; }
        IAppVentasRepository AppVentasRepository { get; }
        IPcTipoPagoRepository PcTipoPagoRepository { get; }
        IPcProcesoRepository PcProcesoRepository { get; }
        ISapEntregasFiscalesRepository SapEntregasFiscalesRepository { get; }
        IAppOrdenProductoRepeticionRepository AppOrdenProductoRepeticionRepository { get; }

        ICobAdjuntosCobranzaRepository CobAdjuntosCobranzaRepository { get; }

        ICobGeneralCobranzaRepository GeneralCobranzaRepository { get; }
        ICreSolicitudExcepcionRepository CreSolicitudExcepcionRepository { get; }
        
        

        ISegUsuarioRepository SegUsuarioRepository { get; }

        IMtrClienteRepository MtrClienteRepository { get; }
        IWsmy264Repository Wsmy264Repository { get; }

        ICobTipoTransaccionRepository CobTipoTransaccionRepository { get; }

        IOfdTipoDocumentoRepository OfdTipoDocumentoRepository { get; }

        IMtrOficinaRepository MtrOficinaRepository { get; }

        IMtrVendedorRepository MtrVendedorRepository { get; }

        IMtrBancosRepository MtrBancosRepository { get; }

        IMtrTipoMonedaRepository MtrTipoMonedaRepository { get; }

        ICobGrabacionCobranzasRepository CobGrabacionCobranzasRepository { get; }

        IMtrTipoTransaccionCuentaDestinoRepository MtrTipoTransaccionCuentaDestinoRepository { get; }

        ICobTransaccionesRepository CobTransaccionesRepository { get; }

        ICobEstadoDeCuentaRepository CobEstadoDeCuentaRepository { get; }

        IOfdCotizacionRepository OfdCotizacionRepository { get; }

        IOfdAdjuntoRepository OfdAdjuntoRepository { get; }

        IOfdAdjuntoCriterioRepository OfdAdjuntoCriterioRepository { get; }

         ICobPagosRetencionesRepository CobPagosRetencionesRepository { get; }

        ICobSysfileRepository CobSysfileRepository { get; }

        ISapRetencionesRepository SapRetencionesRepository { get; }

        ICobValoresRetencionesRepository CobValoresRetencionesRepository { get; }

        IMtrContactosRepository MtrContactosRepository { get; }

        ITPaTasaBancoCentralRepository TPaTasaBancoCentralRepository { get; }

        ICobMatrixMonedaValidaPagoRepository CobMatrixMonedaValidaPagoRepository { get; }

        IPageMenuRepository PageMenuRepository { get; }

        IChildMenuRepository ChildMenuRepository { get; }

        IAppGeneralQuotesRepository AppGeneralQuotesRepository { get; }

        IMtrDireccionesRepository MtrDireccionesRepository { get; }

        IMtrCondicionPagoRepository MtrCondicionPagoRepository { get; }

        IAppStatusQuoteRepository AppStatusQuoteRepository { get; }

        IAppUnitsRepository AppUnitsRepository { get; }

        IAppProductConversionRepository AppProductConversionRepository { get; }

        IAppSubCategoryRepository AppSubCategoryRepository { get; }

        IAppProductsRepository AppProductsRepository { get; }

        IAppDetailQuotesRepository AppDetailQuotesRepository { get; }

        IAppVariablesRepository AppVariablesRepository { get; }

        IAppIngredientsRepository AppIngredientsRepository { get; }

        IAppRecipesRepository AppRecipesRepository { get; }

        IAppConfigAppRepository AppConfigAppRepository { get; }

        IAppVariableSearchRepository AppVariableSearchRepository { get; }

        IAppRecipesByAppDetailQuotesRepository AppRecipesByAppDetailQuotesRepository { get; }
        IAppProductVariableSearchTextRepository AppProductVariableSearchTextRepository { get; }

        IAppTemplateConversionUnitRepository AppTemplateConversionUnitRepository { get; }


        IAppDetailQuotesConversionUnitRepository AppDetailQuotesConversionUnitRepository { get; }


        ITPaTasaReferencialRepository TPaTasaReferencialRepository { get; }

        ITEnControlCalculoRepository TEnControlCalculoRepository { get; }
        IAppPriceRepository AppPriceRepository { get; }

        IAppPorcentajeAdicionalM2Repository AppPorcentajeAdicionalM2Repository { get; }

        IAppTituloVariableRepository AppTituloVariableRepository { get; }
        IAppCostosEmbaqueRepository AppCostosEmbaqueRepository { get; }

        // Nomina
        INmm024Repository Nmm024Repository { get; }
        INmt033Repository Nmt033Repository { get; }

        //Mooreve
        IWpry254Repository Wpry254Repository { get; }
        IMotivoGanarPerderRepository MotivoGanarPerderRepository { get; }

        ICompetidoresRepository CompetidoresRepository { get; }

        ICotizacionRepository CotizacionRepository { get; }

        IRenglonRepository RenglonRepository { get; }

        IPropuestaRepository PropuestaRepository { get; }

        IAprobacionesRepository AprobacionesRepository { get; }

        IWsmy649Repository Wsmy649Repository { get; }

        IWsmy647Repository Wsmy647Repository { get; }

        IWsmy655Repository Wsmy655Repository { get; }

        IWsmy645Repository Wsmy645Repository { get; }

        IWpry229Repository Wpry229Repository { get; }

        IWsmy406Repository Wsmy406Repository { get; }

        IWpry240Repository Wpry240Repository { get; }

        IWpry241Repository Wpry241Repository { get; }


        IWSMY582Repository WSMY582Repository { get; }

        IWSMY583Repository WSMY583Repository { get; }

        IWsmy501EnviarOdooRepository Wsmy501EnviarOdooRepository { get; }
        IWsmy501BorradosRepository Wsmy501BorradosRepository { get; }
        IWsmy369Repository Wsmy369Repository { get; }
        IWpry243Repository Wpry243Repository { get; }
        IWpry249Repository Wpry249Repository { get; }
        IWpry251Repository Wpry251Repository { get; }
        IVCotizacionesPorAprobarRepository VCotizacionesPorAprobarRepository { get; }
        ICobEstadoDeCuentaMultimonedaRepository CobEstadoDeCuentaMultimonedaRepository { get; }

        IAppValoresVariablesEspecificacionesPartesRepository AppValoresVariablesEspecificacionesPartesRepository { get; }
        IAppVariablesEspecificacionesPartesRepository AppVariablesEspecificacionesPartesRepository { get; }
        IAppDesarrolloEtiquetasPrimeRepository AppDesarrolloEtiquetasPrimeRepository { get; }
        IAppReporteCotizacionEtiquetasPrimeRepository AppReporteCotizacionEtiquetasPrimeRepository { get; }

        IWiny244Repository Winy244Repository { get; }
        
        IAppAdjuntosCotizacionRepository AppAdjuntosCotizacionRepository { get; }
        
        
        IWsmy686Repository Wsmy686Repository { get; }
        IVHistoricoComisionesRepository VHistoricoComisionesRepository { get; }
        IPcPagosManualesRepository PcPagosManualesRepository { get; }
        
        
        ITasaConsolidadoRepository TasaConsolidadoRepository { get; }
        //Maestros

        ICsmy036Repository Csmy036Repository { get; }
        ICsmy005Repository Csmy005Repository { get; }


        //Sap
        ISapTratamientoContactoRepository SapTratamientoContactoRepository { get; }
        ISapCargoContactoRepository SapCargoContactoRepository { get; }
        ISapDepartamentoContactoRepository SapDepartamentoContactoRepository { get; }
        ISapPoderContactoRepository SapPoderContactoRepository { get; }
        ISapContactoRepository SapContactoRepository { get; }


        //Clientes
        IWsmy265Repository Wsmy265Repository { get; }
        IWsmy065Repository Wsmy065Repository { get; }

        //Facturacion

        IWiny243Repository Winy243Repository { get; }
        ICiny057Repository Ciny057Repository { get; }
        ICiny056Repository Ciny056Repository { get; }
        //Contratos stock
        IDatosClienteRepository DatosClienteRepository { get; }
        IDatosProductosRepository DatosProductosRepository { get; }

        //DW
        IDwConsolidadoAnalisisDeVencimientoRepository DwConsolidadoAnalisisDeVencimientoRepository { get; }

        //Spi
        ISpiNmm024Repository SpiNmm024Repository { get; }
        ISpiNmt033Repository SpiNmt033Repository { get; }



        //Planta

        ICpry012Repository Cpry012Repository { get; }
        ICsmy021Repository Csmy021Repository { get; }

        //Materiales
        IWimy001Repository Wimy001Repository { get; }

        //Estadisticas
        IVentasRepository VentasRepository { get; }


        bool SaveChanges();

        Task<bool> SaveChangesAsync();



    }
}
