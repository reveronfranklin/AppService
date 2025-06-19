
using AAppService.Core.EntitiesPowerBI;
using AppService.Core.EntitiesPowerBI;
using Microsoft.EntityFrameworkCore;


namespace AppService.Infrastructure.DataPowerBI
{
    public partial class POWERBIContext : DbContext
    {
        public POWERBIContext()
        {
        }

        public POWERBIContext(DbContextOptions<POWERBIContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CobRangoDiasPago> CobRangoDiasPagos { get; set; }
        public virtual DbSet<Cobro> Cobros { get; set; }
        public virtual DbSet<CobrosDiasTranscurrido> CobrosDiasTranscurridos { get; set; }
        public virtual DbSet<ContabilidadCebesYCeco> ContabilidadCebesYCecos { get; set; }
        public virtual DbSet<CotizacionesEnTuberium> CotizacionesEnTuberia { get; set; }
        public virtual DbSet<Descuentoporme> Descuentopormes { get; set; }
        public virtual DbSet<Embarque> Embarques { get; set; }
        public virtual DbSet<HistoricoComisione> HistoricoComisiones { get; set; }
        public virtual DbSet<Ordene> Ordenes { get; set; }
        public virtual DbSet<OrdenesEstrategica> OrdenesEstrategicas { get; set; }
        public virtual DbSet<RangoVariacionVentaMinimo> RangoVariacionVentaMinimos { get; set; }
        public virtual DbSet<TNominaVenta> TNominaVentas { get; set; }
        public virtual DbSet<TemporalComisione> TemporalComisiones { get; set; }
        public virtual DbSet<VAppCostosEmbarque> VAppCostosEmbarques { get; set; }
        public virtual DbSet<VBacklogCpry010> VBacklogCpry010s { get; set; }
        public virtual DbSet<VCobEstadoCuentaMultiMonedum> VCobEstadoCuentaMultiMoneda { get; set; }
        public virtual DbSet<VCotizacionesEnTuberium> VCotizacionesEnTuberia { get; set; }
        public virtual DbSet<VCotizadoVendidoCobrado> VCotizadoVendidoCobrados { get; set; }
        public virtual DbSet<VDwConsolidadoAnalisisDeVencimiento> VDwConsolidadoAnalisisDeVencimientos { get; set; }
        public virtual DbSet<VEmbarquesVenta> VEmbarquesVentas { get; set; }
        public virtual DbSet<VEstadisticasEmbarquesLinea> VEstadisticasEmbarquesLineas { get; set; }
        public virtual DbSet<VHistoricoComisionesLinealesMercaderium> VHistoricoComisionesLinealesMercaderia { get; set; }
        public virtual DbSet<VHistoricoComsione> VHistoricoComsiones { get; set; }
        public virtual DbSet<VNominaVenta> VNominaVentas { get; set; }
        public virtual DbSet<VPendienteAprobacionCxc> VPendienteAprobacionCxcs { get; set; }
        public virtual DbSet<VPendienteAprobacionPlantum> VPendienteAprobacionPlanta { get; set; }
        public virtual DbSet<VRetencione> VRetenciones { get; set; }
        public virtual DbSet<VRetencionesCobrada> VRetencionesCobradas { get; set; }
        public virtual DbSet<VSolicitudesExcepcionPendiente> VSolicitudesExcepcionPendientes { get; set; }
        public virtual DbSet<VTemporalComisione> VTemporalComisiones { get; set; }
        public virtual DbSet<VTemporalComsione> VTemporalComsiones { get; set; }
        public virtual DbSet<VTiemposActualizacionEstadoCuentaMultimonedum> VTiemposActualizacionEstadoCuentaMultimoneda { get; set; }
        public virtual DbSet<VTiemposWepy208> VTiemposWepy208s { get; set; }
        public virtual DbSet<VVenta> VVentas { get; set; }
        public virtual DbSet<Ventas> Ventas { get; set; }
        public virtual DbSet<VVtascuota> VVtascuotas { get; set; }
        public virtual DbSet<Vventa1> Vventas1 { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=172.28.107.19\\FSVEMCYN03D;Database=POWERBI;User Id=userweb;Password=userweb2003;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CobRangoDiasPago>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CobRangoDiasPago");

                entity.Property(e => e.Titulo).HasMaxLength(200);
            });

            modelBuilder.Entity<Cobro>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.Orden, "IX_Cobros");

                entity.HasIndex(e => new { e.AloContable, e.MesContable }, "IX_Cobros_1");

                entity.Property(e => e.AloContable).HasColumnName("Alo_Contable");

                entity.Property(e => e.Base).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Categoria).HasMaxLength(50);

                entity.Property(e => e.CodOficina).HasColumnName("COD_OFICINA");

                entity.Property(e => e.Concepto).HasMaxLength(200);

                entity.Property(e => e.Documento).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.DocumentoAfecta).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.DolarBcv)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("DolarBCV");

                entity.Property(e => e.DolarParalelo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DolarParaleloOrden).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DolarPromedio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.FechaIngreso).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Impuesto).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MesContable).HasColumnName("Mes_Contable");

                entity.Property(e => e.Moneda).HasMaxLength(50);

                entity.Property(e => e.Monto).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NomOficina)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOM_OFICINA");

                entity.Property(e => e.NomVendedor)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("NOM_VENDEDOR")
                    .IsFixedLength();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE")
                    .IsFixedLength();

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION");

                entity.Property(e => e.PorcentajeIva).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Region)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("REGION");

                entity.Property(e => e.RegionJunta)
                    .HasMaxLength(50)
                    .HasColumnName("REGION_JUNTA");

                entity.Property(e => e.SubCategoria).HasMaxLength(50);

                entity.Property(e => e.TasaBcv)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("TasaBCV");

                entity.Property(e => e.TasaParalelo).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.TasaParaleloOrden).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.TasaPromedio).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Transaccion).HasMaxLength(2);

                entity.Property(e => e.TransaccionAfecta).HasMaxLength(2);

                entity.Property(e => e.UsdVentaOrden).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<CobrosDiasTranscurrido>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Banco)
                    .IsRequired()
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CodOficina)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CodigoVendedor)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Cotizacion)
                    .HasMaxLength(13)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DocAfecta).HasColumnType("decimal(18, 0)");

                entity.Property(e => e.DocAfectaSap).HasMaxLength(10);

                entity.Property(e => e.DocumentoSap).HasMaxLength(10);

                entity.Property(e => e.EjercicioDocumentoSap).HasMaxLength(4);

                entity.Property(e => e.FacturaNd)
                    .HasMaxLength(20)
                    .HasColumnName("FacturaND");

                entity.Property(e => e.FechaLm)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("FechaLM");

                entity.Property(e => e.FechaOrden).HasColumnType("smalldatetime");

                entity.Property(e => e.FechaRecibidoCliente).HasColumnType("smalldatetime");

                entity.Property(e => e.FechaRegistro).HasColumnType("smalldatetime");

                entity.Property(e => e.FechaTasa).HasColumnType("smalldatetime");

                entity.Property(e => e.FechaTransaccion).HasColumnType("smalldatetime");

                entity.Property(e => e.IdCliente)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.IdTipoTransaccion)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(e => e.Moneda).HasMaxLength(50);

                entity.Property(e => e.Monto).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoBs).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.MontoUsd).HasColumnType("decimal(37, 6)");

                entity.Property(e => e.NombreCliente).HasMaxLength(80);

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION");

                entity.Property(e => e.NombreVendedor)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false);

                entity.Property(e => e.Oficina)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Rpdct)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("RPDCT")
                    .IsFixedLength();

                entity.Property(e => e.Rpdoc).HasColumnName("RPDOC");

                entity.Property(e => e.Rpkco)
                    .IsRequired()
                    .HasMaxLength(5)
                    .HasColumnName("RPKCO")
                    .IsFixedLength();

                entity.Property(e => e.Rpsfx)
                    .IsRequired()
                    .HasMaxLength(3)
                    .HasColumnName("RPSFX")
                    .IsFixedLength();

                entity.Property(e => e.SoloBs).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SoloUsd).HasColumnType("decimal(37, 6)");

                entity.Property(e => e.TasaBancoCentral).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.TasaMonitor).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Titulo).HasMaxLength(200);

                entity.Property(e => e.TotalVentaOrden).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Transaccion).HasMaxLength(2);

                entity.Property(e => e.Usdmonitor)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("USDMonitor");
            });

            modelBuilder.Entity<ContabilidadCebesYCeco>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Contabilidad CEBES y CECOS");

                entity.HasIndex(e => new { e.Ano, e.Mes }, "IX_Contabilidad CEBES y CECOS");

                entity.HasIndex(e => new { e.Ano, e.Mes, e.Cuenta }, "IX_Contabilidad CEBES y CECOS_1");

                entity.HasIndex(e => new { e.Ano, e.Mes, e.Cebes }, "IX_Contabilidad CEBES y CECOS_2");

                entity.HasIndex(e => new { e.Ano, e.Mes, e.ClasificacionMadre, e.TipoCuenta, e.Cebes, e.Cecos }, "IX_Contabilidad CEBES y CECOS_3");

                entity.HasIndex(e => e.Ano, "IX_Contabilidad CEBES y CECOS_4");

                entity.Property(e => e.Ano).HasColumnName("ANO");

                entity.Property(e => e.Cebes)
                    .HasMaxLength(50)
                    .HasColumnName("CEBES");

                entity.Property(e => e.Cecos)
                    .HasMaxLength(50)
                    .HasColumnName("CECOS");

                entity.Property(e => e.ClasificacionMadre)
                    .HasMaxLength(10)
                    .HasColumnName("CLASIFICACION_MADRE")
                    .IsFixedLength();

                entity.Property(e => e.Cuenta).HasColumnName("CUENTA");

                entity.Property(e => e.DescripcionCuenta)
                    .HasMaxLength(255)
                    .HasColumnName("DESCRIPCION CUENTA");

                entity.Property(e => e.Mes).HasColumnName("MES");

                entity.Property(e => e.Monto).HasColumnName("MONTO");

                entity.Property(e => e.Tasa).HasColumnName("TASA");

                entity.Property(e => e.TipoCuenta)
                    .HasMaxLength(50)
                    .HasColumnName("TIPO_CUENTA")
                    .IsFixedLength();

                entity.Property(e => e.Usd).HasColumnName("USD");
            });

            modelBuilder.Entity<CotizacionesEnTuberium>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("COTIZACIONES_EN_TUBERIA");

                entity.HasIndex(e => new { e.Ano, e.Mes }, "IX_COTIZACIONES_EN_TUBERIA");

                entity.HasIndex(e => new { e.NomOficina, e.Vendedor }, "IX_COTIZACIONES_EN_TUBERIA_1");

                entity.HasIndex(e => e.Descripcion, "IX_COTIZACIONES_EN_TUBERIA_2");

                entity.Property(e => e.Cliente)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CodCliente)
                    .HasMaxLength(10)
                    .HasColumnName("Cod_Cliente");

                entity.Property(e => e.CodOficina).HasColumnName("COD_OFICINA");

                entity.Property(e => e.CodProducto)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Cod_Producto");

                entity.Property(e => e.CodVendedor)
                    .HasMaxLength(4)
                    .HasColumnName("Cod_Vendedor");

                entity.Property(e => e.CodigoRegion)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CODIGO_REGION");

                entity.Property(e => e.Codmos)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CODMOS");

                entity.Property(e => e.Competidor)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CondicionPago)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cotizacion).HasMaxLength(13);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DescripcionVariacion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FechaActualiza).HasColumnType("datetime");

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.LineaNegocio).HasMaxLength(100);

                entity.Property(e => e.Motivo).HasMaxLength(30);

                entity.Property(e => e.NomOficina)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOM_OFICINA");

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION");

                entity.Property(e => e.PorcVentasMaximo).HasColumnType("numeric(16, 2)");

                entity.Property(e => e.PorcVentasMinimo).HasColumnType("numeric(16, 2)");

                entity.Property(e => e.PrecioMaximo)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("precio_maximo");

                entity.Property(e => e.PrecioMinimo).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Producto)
                    .HasMaxLength(65)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ProductoCuota).HasMaxLength(50);

                entity.Property(e => e.RazonSocial)
                    .HasMaxLength(80)
                    .HasColumnName("Razon_Social");

                entity.Property(e => e.SubCategoria)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalPropuestaUsd).HasColumnType("numeric(20, 4)");

                entity.Property(e => e.Vendedor)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Descuentoporme>(entity =>
            {
                entity.ToTable("DESCUENTOPORMES");

                entity.HasIndex(e => new { e.AloContable, e.MesContable }, "IX_DESCUENTOPORMES");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.AloContable).HasColumnName("ALO_CONTABLE");

                entity.Property(e => e.MesContable).HasColumnName("MES_CONTABLE");

                entity.Property(e => e.Porcentaje)
                    .HasMaxLength(10)
                    .HasColumnName("PORCENTAJE")
                    .IsFixedLength();
            });

            modelBuilder.Entity<Embarque>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.AloContable).HasColumnName("Alo_Contable");

                entity.Property(e => e.Base).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Cantidad).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Categoria).HasMaxLength(50);

                entity.Property(e => e.CodOficina).HasColumnName("COD_OFICINA");

                entity.Property(e => e.Documento).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.DocumentoAfecta).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.DolarBcv)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("DolarBCV");

                entity.Property(e => e.DolarParalelo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DolarPromedio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FactAdelantada)
                    .HasMaxLength(1)
                    .HasColumnName("FACT_ADELANTADA");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Impuesto).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.MesContable).HasColumnName("Mes_Contable");

                entity.Property(e => e.Monto).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NomOficina)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOM_OFICINA");

                entity.Property(e => e.NomVendedor)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("NOM_VENDEDOR")
                    .IsFixedLength();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE")
                    .IsFixedLength();

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION");

                entity.Property(e => e.PorcentajeIva).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Region)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("REGION");

                entity.Property(e => e.RegionJunta)
                    .HasMaxLength(50)
                    .HasColumnName("REGION_JUNTA");

                entity.Property(e => e.SubCategoria).HasMaxLength(50);

                entity.Property(e => e.TasaBcv)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("TasaBCV");

                entity.Property(e => e.TasaParalelo).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.TasaPromedio).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Transaccion).HasMaxLength(2);

                entity.Property(e => e.TransaccionAfecta).HasMaxLength(2);
            });

            modelBuilder.Entity<HistoricoComisione>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.IdCliente, "IX_HistoricoComisiones");

                entity.HasIndex(e => e.IdVendedor, "IX_HistoricoComisiones_1");

                entity.HasIndex(e => e.Periodo, "IX_HistoricoComisiones_10");

                entity.HasIndex(e => e.IdPeriodo, "IX_HistoricoComisiones_11");

                entity.HasIndex(e => e.NombreOficina, "IX_HistoricoComisiones_2");

                entity.HasIndex(e => e.NombreVendedor, "IX_HistoricoComisiones_3");

                entity.HasIndex(e => new { e.Ano, e.Mes }, "IX_HistoricoComisiones_4");

                entity.HasIndex(e => e.Producto, "IX_HistoricoComisiones_5");

                entity.HasIndex(e => e.ProductoDescripcion, "IX_HistoricoComisiones_6");

                entity.HasIndex(e => e.Subcategoria, "IX_HistoricoComisiones_7");

                entity.HasIndex(e => e.Concepto, "IX_HistoricoComisiones_8");

                entity.HasIndex(e => new { e.Ano, e.Mes, e.Subcategoria }, "IX_HistoricoComisiones_9");

                entity.Property(e => e.BsBaseComsion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BsComision).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CantidadOrden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ComCuotasCump).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ComCuotasGenerales).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Concepto).HasMaxLength(100);

                entity.Property(e => e.Concesion).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DescripcionPeriodo).HasMaxLength(200);

                entity.Property(e => e.DescripcionTipoPago).HasMaxLength(255);

                entity.Property(e => e.FechaOrden).HasColumnType("datetime");

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.FechaTasa).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IdCliente).HasMaxLength(10);

                entity.Property(e => e.IdVendedor).HasMaxLength(50);

                entity.Property(e => e.LineaNegocio).HasMaxLength(255);

                entity.Property(e => e.MontoRecibo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NombreCliente).HasMaxLength(255);

                entity.Property(e => e.NombreOficina).HasMaxLength(255);

                entity.Property(e => e.NombreVendedor).HasMaxLength(255);

                entity.Property(e => e.Nuevacomision).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ObsManuales).HasMaxLength(500);

                entity.Property(e => e.Orden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Periodo).HasMaxLength(100);

                entity.Property(e => e.PorComisionAplicado).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Porcentaje).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Producto).HasMaxLength(12);

                entity.Property(e => e.ProductoDescripcion).HasMaxLength(255);

                entity.Property(e => e.SearchText).HasMaxLength(4000);

                entity.Property(e => e.Sobreprecio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Subcategoria).HasMaxLength(255);

                entity.Property(e => e.Tasa).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UsdBaseComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionCantidadCuotasCumplidas).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionRangoCumplimientoCuotaGeneral).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.VentaOrdenUsd).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<Ordene>(entity =>
            {
                entity.HasKey(e => e.Orden)
                    .HasName("PK_Orden");

                entity.Property(e => e.Orden).ValueGeneratedNever();

                entity.Property(e => e.AloContable).HasColumnName("ALO_CONTABLE");

                entity.Property(e => e.Cantidad).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CantidadEmbarcada).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Categoria).HasMaxLength(50);

                entity.Property(e => e.CodOficina).HasColumnName("COD_OFICINA");

                entity.Property(e => e.Cotizacion).HasMaxLength(13);

                entity.Property(e => e.Descuento).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiferenciaCantidad).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DiferenciaFacturadoBsCobradoBs).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DolarBcv)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("DolarBCV");

                entity.Property(e => e.DolarParalelo).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.DolarPromedio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.FechaContableUltimoCobro).HasColumnType("datetime");

                entity.Property(e => e.FechaEmisionUltimoCobro).HasColumnType("datetime");

                entity.Property(e => e.FechaTasaReferencial).HasColumnType("datetime");

                entity.Property(e => e.MesContable).HasColumnName("MES_CONTABLE");

                entity.Property(e => e.NomOficina)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOM_OFICINA");

                entity.Property(e => e.NomVendedor)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("NOM_VENDEDOR")
                    .IsFixedLength();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE")
                    .IsFixedLength();

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION");

                entity.Property(e => e.PrecioMinimo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Region)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("REGION");

                entity.Property(e => e.RegionJunta)
                    .HasMaxLength(50)
                    .HasColumnName("REGION_JUNTA");

                entity.Property(e => e.SubCategoria).HasMaxLength(50);

                entity.Property(e => e.TasaBcv)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("TasaBCV");

                entity.Property(e => e.TasaParalelo).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.TasaPromedio).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.TotalCobrado).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalCobradoBcv)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("TotalCobradoBCV");

                entity.Property(e => e.TotalCobradoBs).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalCobradoParalelo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalCobradoPromedio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalFacturadoBs).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalPropuestaUsd).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TotalPropuestaUsdMinimo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Venta).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<OrdenesEstrategica>(entity =>
            {
                entity.HasIndex(e => e.Orden, "IX_OrdenesEstrategicas")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<RangoVariacionVentaMinimo>(entity =>
            {
                entity.ToTable("RangoVariacionVentaMinimo");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.RangoDesde).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.RangoHasta).HasColumnType("decimal(18, 4)");
            });

            modelBuilder.Entity<TNominaVenta>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("T_NominaVentas");

                entity.Property(e => e.Apell1)
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("apell1");

                entity.Property(e => e.Apell2)
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("apell2");

                entity.Property(e => e.Bssigno)
                    .HasColumnType("numeric(32, 9)")
                    .HasColumnName("BSSIGNO");

                entity.Property(e => e.CategoriaConcepto).HasMaxLength(50);

                entity.Property(e => e.CtoCodcto)
                    .HasColumnType("numeric(4, 0)")
                    .HasColumnName("CTO_CODCTO");

                entity.Property(e => e.Descto)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("DESCTO");

                entity.Property(e => e.Desdep)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("DESDEP");

                entity.Property(e => e.Dolares)
                    .HasColumnType("numeric(38, 13)")
                    .HasColumnName("DOLARES");

                entity.Property(e => e.DptoCoddep)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DPTO_CODDEP");

                entity.Property(e => e.Fecmov)
                    .HasColumnType("datetime")
                    .HasColumnName("FECMOV");

                entity.Property(e => e.Fecpag).HasColumnName("FECPAG");

                entity.Property(e => e.Fecsis)
                    .HasColumnType("datetime")
                    .HasColumnName("FECSIS");

                entity.Property(e => e.FproAnocal)
                    .HasColumnType("numeric(4, 0)")
                    .HasColumnName("FPRO_ANOCAL");

                entity.Property(e => e.Mescal)
                    .HasColumnType("numeric(2, 0)")
                    .HasColumnName("MESCAL");

                entity.Property(e => e.Moncto)
                    .HasColumnType("numeric(21, 9)")
                    .HasColumnName("MONCTO");

                entity.Property(e => e.Nombr1)
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("nombr1");

                entity.Property(e => e.Nombr2)
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("nombr2");

                entity.Property(e => e.Nosuma)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("NOSUMA");

                entity.Property(e => e.ProcTippro)
                    .HasColumnType("numeric(2, 0)")
                    .HasColumnName("PROC_TIPPRO")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Subpro)
                    .HasColumnType("numeric(1, 0)")
                    .HasColumnName("SUBPRO")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Tasa).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TnomTipnom)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("TNOM_TIPNOM");

                entity.Property(e => e.TrabFictra)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TRAB_FICTRA");
            });

            modelBuilder.Entity<TemporalComisione>(entity =>
            {
                entity.HasNoKey();

                entity.HasIndex(e => e.IdCliente, "IX_TemporalComisiones");

                entity.HasIndex(e => e.NombreVendedor, "IX_TemporalComisiones_1");

                entity.HasIndex(e => e.ProductoDescripcion, "IX_TemporalComisiones_2");

                entity.HasIndex(e => e.Subcategoria, "IX_TemporalComisiones_3");

                entity.Property(e => e.BsBaseComsion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BsComision).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CantidadOrden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ComCuotasCump).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ComCuotasGenerales).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Concepto).HasMaxLength(100);

                entity.Property(e => e.Concesion).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DescripcionTipoPago).HasMaxLength(255);

                entity.Property(e => e.FechaOrden).HasColumnType("datetime");

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.FechaTasa).HasColumnType("datetime");

                entity.Property(e => e.IdCliente).HasMaxLength(10);

                entity.Property(e => e.IdVendedor).HasMaxLength(50);

                entity.Property(e => e.LineaNegocio).HasMaxLength(255);

                entity.Property(e => e.MontoRecibo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NombreCliente).HasMaxLength(255);

                entity.Property(e => e.NombreOficina).HasMaxLength(255);

                entity.Property(e => e.NombreVendedor).HasMaxLength(255);

                entity.Property(e => e.Orden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.PorcComisiones).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Porcentaje).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Producto).HasMaxLength(12);

                entity.Property(e => e.ProductoDescripcion).HasMaxLength(255);

                entity.Property(e => e.Sobreprecio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Subcategoria).HasMaxLength(255);

                entity.Property(e => e.Tasa).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UsdBaseComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionCantidadCuotasCumplidas).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionRangoCumplimientoCuotaGeneral).HasColumnType("decimal(20, 2)");
            });

            modelBuilder.Entity<VAppCostosEmbarque>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_AppCostosEmbarques");

                entity.Property(e => e.Articulo)
                    .HasMaxLength(12)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Categoria)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CodRegion)
                    .HasMaxLength(2)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CodVendedor)
                    .HasMaxLength(4)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CodeVariable)
                    .HasMaxLength(100)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Cotizacion)
                    .HasMaxLength(13)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DescripcionDocumento)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.Id).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Monto).HasColumnType("numeric(21, 2)");

                entity.Property(e => e.NombreCliente)
                    .HasMaxLength(200)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreOficina)
                    .HasMaxLength(100)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreRegion1)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreVededor)
                    .HasMaxLength(100)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.OrigenCotizacion)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.PoncertajeFacturado).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Signo).HasColumnType("numeric(2, 0)");

                entity.Property(e => e.SubCategoria)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.TipoDocumento)
                    .HasMaxLength(2)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Titulo)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.TituloCostoEmbarque)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Valor).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.ValorFacturado).HasColumnType("numeric(21, 4)");

                entity.Property(e => e.ValorFacturadoConSigno).HasColumnType("numeric(23, 4)");

                entity.Property(e => e.VentaCotizacion).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.VentaOrden).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<VBacklogCpry010>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_BAcklogCpry010");

                entity.Property(e => e.AbreviadoMaquinaColectora)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.AbreviadoMaquinaPrensa)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CantFactura).HasColumnName("CANT_FACTURA");

                entity.Property(e => e.CantProgramada).HasColumnName("CANT_PROGRAMADA");

                entity.Property(e => e.Categoria)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CodProducto)
                    .IsRequired()
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("COD_PRODUCTO")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DiasProduccionColectora).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.DiasProduccionPrensa).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.Estacion)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.HorasPTurnoColectora)
                    .HasColumnType("decimal(6, 2)")
                    .HasColumnName("Horas P/Turno Colectora");

                entity.Property(e => e.HorasPTurnoPrensa)
                    .HasColumnType("decimal(6, 2)")
                    .HasColumnName("Horas P/Turno Prensa");

                entity.Property(e => e.HorasTiempoProduccionColectora).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.HorasTiempoProduccionPrensa).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.JobRetenido)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false);

                entity.Property(e => e.MaquinaColectora)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.MaquinaPrensa)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreCliente)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Oficina)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.PiesTotalesEstacion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PiesXhoraRealColectora)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("PiesXHoraReal Colectora");

                entity.Property(e => e.PiesXhoraRealPrensa)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("PiesXHoraReal Prensa");

                entity.Property(e => e.ProgramadaFacturada).HasColumnName("PROGRAMADA-Facturada");

                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.SubCategoria)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.TipoJob)
                    .HasMaxLength(9)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.TotalPropuestaUsd).HasColumnType("numeric(20, 4)");

                entity.Property(e => e.UsdCantidadEstacion).HasColumnType("numeric(38, 6)");

                entity.Property(e => e.Vendedor)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");
            });

            modelBuilder.Entity<VCobEstadoCuentaMultiMonedum>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_CobEstadoCuentaMultiMoneda");

                entity.Property(e => e.BaseImponible).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.BaseImponibleUsd).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.ClaseDoc)
                    .IsRequired()
                    .HasMaxLength(2)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Cliente)
                    .HasMaxLength(10)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CodigoRegion)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("Codigo_Region")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Cotizacion)
                    .HasMaxLength(13)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DebeHaber)
                    .HasMaxLength(1)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DescripcionClaseDocumento)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DocumentoExterno)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DocumentoSap)
                    .HasMaxLength(50)
                    .HasColumnName("DocumentoSAP")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.FechaActualBancoCentralBaseImponibleUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_BANCO_CENTRAL_BaseImponibleUsd");

                entity.Property(e => e.FechaActualBancoCentralImpuestoUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_BANCO_CENTRAL_ImpuestoUsd");

                entity.Property(e => e.FechaActualBancoCentralMontoUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_BANCO_CENTRAL_MontoUsd");

                entity.Property(e => e.FechaActualBancoCentralRetencionPendienteUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_BANCO_CENTRAL_RetencionPendienteUsd");

                entity.Property(e => e.FechaActualBancoCentralSaldoUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_BANCO_CENTRAL_SaldoUsd");

                entity.Property(e => e.FechaActualBancoCentralTasa)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_BANCO_CENTRAL_Tasa");

                entity.Property(e => e.FechaActualReferencialBaseImponibleUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_BaseImponibleUsd");

                entity.Property(e => e.FechaActualReferencialCotizacionBaseImponibleUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_COTIZACION_BaseImponibleUsd");

                entity.Property(e => e.FechaActualReferencialCotizacionImpuestoUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_COTIZACION_ImpuestoUsd");

                entity.Property(e => e.FechaActualReferencialCotizacionMontoUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_COTIZACION_MontoUsd");

                entity.Property(e => e.FechaActualReferencialCotizacionRetencionPendienteUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_COTIZACION_RetencionPendienteUsd");

                entity.Property(e => e.FechaActualReferencialCotizacionSaldoUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_COTIZACION_SaldoUsd");

                entity.Property(e => e.FechaActualReferencialCotizacionTasa)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_COTIZACION__Tasa");

                entity.Property(e => e.FechaActualReferencialImpuestoUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_ImpuestoUsd");

                entity.Property(e => e.FechaActualReferencialMontoUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_MontoUsd");

                entity.Property(e => e.FechaActualReferencialRetencionPendienteUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_RetencionPendienteUsd");

                entity.Property(e => e.FechaActualReferencialSaldoUsd)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_SaldoUsd");

                entity.Property(e => e.FechaActualReferencialTasa)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("FECHA_ACTUAL_REFERENCIAL_Tasa");

                entity.Property(e => e.FechaDoc).HasColumnType("smalldatetime");

                entity.Property(e => e.FechaVencimiento).HasColumnType("smalldatetime");

                entity.Property(e => e.Fin).HasColumnType("datetime");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Impuesto).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.ImpuestoUsd).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Inicio).HasColumnType("datetime");

                entity.Property(e => e.Moneda)
                    .HasMaxLength(3)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Monto).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.MontoUsd).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.NombreCliente)
                    .HasMaxLength(400)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreOficina)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreVendedor)
                    .HasMaxLength(400)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.OficinaVenta)
                    .HasMaxLength(2)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.RetencionPendiente).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.RetencionPendienteUsd).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Saldo).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.SaldoUsd).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Signo)
                    .HasMaxLength(1)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.TasaBancoCentral).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.TasaReferencial).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Texto)
                    .HasMaxLength(100)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Vendedor)
                    .HasMaxLength(4)
                    .UseCollation("Modern_Spanish_CI_AS");
            });

            modelBuilder.Entity<VCotizacionesEnTuberium>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_COTIZACIONES_EN_TUBERIA");

                entity.Property(e => e.Cliente)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CodCliente)
                    .HasMaxLength(10)
                    .HasColumnName("Cod_Cliente");

                entity.Property(e => e.CodOficina).HasColumnName("COD_OFICINA");

                entity.Property(e => e.CodProducto)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Cod_Producto");

                entity.Property(e => e.CodVendedor)
                    .HasMaxLength(4)
                    .HasColumnName("Cod_Vendedor");

                entity.Property(e => e.CodigoRegion)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CODIGO_REGION");

                entity.Property(e => e.Codmos)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CODMOS");

                entity.Property(e => e.Competidor)
                    .HasMaxLength(250)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CondicionPago)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Cotizacion).HasMaxLength(13);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.DescripcionVariacion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.FechaActualiza).HasColumnType("datetime");

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.LineaNegocio).HasMaxLength(100);

                entity.Property(e => e.Motivo).HasMaxLength(30);

                entity.Property(e => e.NomOficina)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOM_OFICINA");

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION");

                entity.Property(e => e.PorcVentasMaximo).HasColumnType("numeric(16, 2)");

                entity.Property(e => e.PorcVentasMinimo).HasColumnType("numeric(16, 2)");

                entity.Property(e => e.PrecioMaximo)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("precio_maximo");

                entity.Property(e => e.PrecioMinimo).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Producto)
                    .HasMaxLength(65)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.ProductoCuota).HasMaxLength(50);

                entity.Property(e => e.RazonSocial)
                    .HasMaxLength(80)
                    .HasColumnName("Razon_Social");

                entity.Property(e => e.SubCategoria)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TotalPropuestaUsd).HasColumnType("numeric(20, 4)");

                entity.Property(e => e.Vendedor)
                    .HasMaxLength(30)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<VCotizadoVendidoCobrado>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_CotizadoVendidoCobrado");

                entity.Property(e => e.ClienteCotizado).HasColumnName("Cliente_Cotizado");

                entity.Property(e => e.CodigoVendedor)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("Codigo_Vendedor")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DolaresCobrado).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.DolaresCotizado)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("Dolares_Cotizado");

                entity.Property(e => e.DolaresVendidos)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("Dolares_Vendidos");

                entity.Property(e => e.FechaRegistro)
                    .HasMaxLength(11)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.NombreOficina)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Nombre_Oficina")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Nombre_Region")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreVendedor)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("Nombre_Vendedor")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NroCotizacion).HasColumnName("Nro_Cotizacion");

                entity.Property(e => e.OrdenesVendidas).HasColumnName("Ordenes_Vendidas");
            });

            modelBuilder.Entity<VDwConsolidadoAnalisisDeVencimiento>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_DwConsolidadoAnalisisDeVencimiento");

                entity.Property(e => e.CodigoCliente).HasColumnName("CODIGO_CLIENTE");

                entity.Property(e => e.CodigoVendedor)
                    .HasMaxLength(6)
                    .HasColumnName("codigo_vendedor")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Documento)
                    .HasMaxLength(25)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.FechaFactura)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA_FACTURA");

                entity.Property(e => e.NombOfic)
                    .HasMaxLength(40)
                    .IsUnicode(false)
                    .HasColumnName("nomb_ofic")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreCliente)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("Nombre_cliente")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreRegion)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreVendedor)
                    .HasMaxLength(80)
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Oficina).HasColumnName("oficina");

                entity.Property(e => e.Retencion).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Tasa).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.TasaHoy)
                    .HasColumnType("numeric(18, 4)")
                    .HasColumnName("Tasa Hoy");
            });

            modelBuilder.Entity<VEmbarquesVenta>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_EmbarquesVentas");

                entity.Property(e => e.Cobrado)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Detalle)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Embarques)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.EstacionCxc)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.EstacionPlanta)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Ventas)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");
            });

            modelBuilder.Entity<VEstadisticasEmbarquesLinea>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_EstadisticasEmbarquesLinea");

                entity.Property(e => e.Articulo)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CantProgramada)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("CANT_PROGRAMADA");

                entity.Property(e => e.Cantidad).HasColumnType("decimal(10, 2)");

                entity.Property(e => e.Cliente)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Codmos)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DescTipo)
                    .HasMaxLength(200)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DetalleCategoria)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.FechaDocumento).HasColumnType("datetime");

                entity.Property(e => e.FechaDolRef)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_dol_ref");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.LineaNegocio)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.LineaProducto)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("LINEA_PRODUCTO")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.McDolRef)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Mc_Dol_Ref");

                entity.Property(e => e.NombreArticulo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Nombre_Articulo")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreCategoria)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreLinea)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("Nombre_Linea")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreSubCategoria)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.PiesCuadrado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PorMcFinan).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.PorcMc).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.RtotalVenta)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("RTotalVenta");

                entity.Property(e => e.TasaDolRef)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("Tasa_Dol_Ref");

                entity.Property(e => e.Tipo)
                    .HasMaxLength(200)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.TotalKgPapel)
                    .HasColumnType("numeric(12, 2)")
                    .HasColumnName("TOTAL_KG_PAPEL");

                entity.Property(e => e.TotalVenta).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.VentaDolRef)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Venta_Dol_Ref");
            });

            modelBuilder.Entity<VHistoricoComisionesLinealesMercaderium>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_HistoricoComisionesLinealesMercaderia");

                entity.Property(e => e.BsBaseComsion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BsComision).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ComCuotasCump).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ComCuotasGenerales).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Descripcion).HasMaxLength(255);

                entity.Property(e => e.DescripcionTipoPago).HasMaxLength(255);

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.FechaTasa).HasColumnType("datetime");

                entity.Property(e => e.IdCliente).HasMaxLength(10);

                entity.Property(e => e.IdVendedor).HasMaxLength(50);

                entity.Property(e => e.LineaNegocio).HasMaxLength(255);

                entity.Property(e => e.NombreCliente).HasMaxLength(255);

                entity.Property(e => e.NombreOficina).HasMaxLength(255);

                entity.Property(e => e.NombreVendedor).HasMaxLength(255);

                entity.Property(e => e.NuevaComision).HasColumnType("numeric(38, 7)");

                entity.Property(e => e.Orden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.PorcentajePorRango).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Producto).HasMaxLength(12);

                entity.Property(e => e.Subcategoria).HasMaxLength(255);

                entity.Property(e => e.Tasa).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UsdBaseComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionCantidadCuotasCumplidas).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionRangoCumplimientoCuotaGeneral).HasColumnType("decimal(20, 2)");
            });

            modelBuilder.Entity<VHistoricoComsione>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_HistoricoComsiones");

                entity.Property(e => e.BsBaseComsion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BsComision).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CantidadOrden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ComCuotasCump).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ComCuotasGenerales)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ComCuotas Generales");

                entity.Property(e => e.Concepto).HasMaxLength(100);

                entity.Property(e => e.Concesion).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Descripcion).HasMaxLength(255);

                entity.Property(e => e.DescripcionPeriodo).HasMaxLength(200);

                entity.Property(e => e.DescripcionTipoPago).HasMaxLength(255);

                entity.Property(e => e.FechaOrden).HasColumnType("datetime");

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.FechaTasa).HasColumnType("datetime");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.IdCliente).HasMaxLength(10);

                entity.Property(e => e.IdVendedor).HasMaxLength(50);

                entity.Property(e => e.LineaNegocio).HasMaxLength(255);

                entity.Property(e => e.MontoRecibo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NombreCliente).HasMaxLength(255);

                entity.Property(e => e.NombreOficina).HasMaxLength(255);

                entity.Property(e => e.NombreVendedor).HasMaxLength(255);

                entity.Property(e => e.Nuevacomision).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ObsManuales).HasMaxLength(500);

                entity.Property(e => e.Orden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Periodo).HasMaxLength(100);

                entity.Property(e => e.PorComisionAplicado).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Porcentaje).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Producto).HasMaxLength(12);

                entity.Property(e => e.SearchText).HasMaxLength(4000);

                entity.Property(e => e.Sobreprecio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Subcategoria).HasMaxLength(255);

                entity.Property(e => e.Tasa).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UsdBaseComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionCantidadCuotasCumplidas).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionRangoCumplimientoCuotaGeneral).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.VentaOrdenUsd).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<VNominaVenta>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_NominaVentas");

                entity.Property(e => e.Apell1)
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("apell1");

                entity.Property(e => e.Apell2)
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("apell2");

                entity.Property(e => e.Bssigno)
                    .HasColumnType("numeric(32, 9)")
                    .HasColumnName("BSSIGNO");

                entity.Property(e => e.CategoriaConcepto).HasMaxLength(50);

                entity.Property(e => e.CtoCodcto)
                    .HasColumnType("numeric(4, 0)")
                    .HasColumnName("CTO_CODCTO");

                entity.Property(e => e.Descto)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("DESCTO");

                entity.Property(e => e.Desdep)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("DESDEP");

                entity.Property(e => e.Dolares)
                    .HasColumnType("numeric(38, 13)")
                    .HasColumnName("DOLARES");

                entity.Property(e => e.DptoCoddep)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("DPTO_CODDEP");

                entity.Property(e => e.Fecmov)
                    .HasColumnType("datetime")
                    .HasColumnName("FECMOV");

                entity.Property(e => e.Fecpag).HasColumnName("FECPAG");

                entity.Property(e => e.Fecsis)
                    .HasColumnType("datetime")
                    .HasColumnName("FECSIS");

                entity.Property(e => e.FproAnocal)
                    .HasColumnType("numeric(4, 0)")
                    .HasColumnName("FPRO_ANOCAL");

                entity.Property(e => e.Mescal)
                    .HasColumnType("numeric(2, 0)")
                    .HasColumnName("MESCAL");

                entity.Property(e => e.Moncto)
                    .HasColumnType("numeric(21, 9)")
                    .HasColumnName("MONCTO");

                entity.Property(e => e.Nombr1)
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("nombr1");

                entity.Property(e => e.Nombr2)
                    .HasMaxLength(17)
                    .IsUnicode(false)
                    .HasColumnName("nombr2");

                entity.Property(e => e.Nosuma)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("NOSUMA");

                entity.Property(e => e.ProcTippro)
                    .HasColumnType("numeric(2, 0)")
                    .HasColumnName("PROC_TIPPRO");

                entity.Property(e => e.Subpro)
                    .HasColumnType("numeric(1, 0)")
                    .HasColumnName("SUBPRO");

                entity.Property(e => e.Tasa).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TnomTipnom)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("TNOM_TIPNOM");

                entity.Property(e => e.TrabFictra)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("TRAB_FICTRA");
            });

            modelBuilder.Entity<VPendienteAprobacionCxc>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_PendienteAprobacionCxc");

                entity.Property(e => e.CodCliente)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("cod_cliente")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CodigoOficina).HasColumnName("codigo_oficina");

                entity.Property(e => e.CodigoProducto)
                    .IsRequired()
                    .HasMaxLength(12)
                    .HasColumnName("codigo_producto")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CodigoVendedor)
                    .IsRequired()
                    .HasMaxLength(4)
                    .HasColumnName("codigo_vendedor")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Cotizacion)
                    .HasMaxLength(13)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .HasColumnName("descripcion")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.MontoSolicitado).HasColumnType("numeric(20, 4)");

                entity.Property(e => e.NomOficina)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("nom_oficina")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreCliente)
                    .HasMaxLength(80)
                    .HasColumnName("nombre_cliente")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreRegion)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("Nombre_Region")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreVendedor)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("nombre_vendedor")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.PorcVentasMinimo).HasColumnType("numeric(38, 13)");

                entity.Property(e => e.PrecioMinimo).HasColumnType("numeric(20, 4)");

                entity.Property(e => e.Producto)
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("producto")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.TotalBs).HasColumnType("numeric(20, 4)");

                entity.Property(e => e.TotalUsd).HasColumnType("numeric(20, 4)");
            });

            modelBuilder.Entity<VPendienteAprobacionPlantum>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_PendienteAprobacionPlanta");

                entity.Property(e => e.CodVend)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Cod_vend")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CodigoClient)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Codigo_Client")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CodigoProducto)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Codigo_producto")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.CotizacionOriginal)
                    .HasMaxLength(18)
                    .HasColumnName("COTIZACION_ORIGINAL")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Estacion)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.FechaPasada)
                    .HasColumnType("smalldatetime")
                    .HasColumnName("Fecha_Pasada");

                entity.Property(e => e.Lineanegocio)
                    .HasMaxLength(100)
                    .HasColumnName("lineanegocio")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NomCliente)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("Nom_Cliente")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NomOficina)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("nom_oficina")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreRegion)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("Nombre_Region")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NumCot)
                    .HasColumnType("numeric(18, 0)")
                    .HasColumnName("Num_Cot");

                entity.Property(e => e.Oficina)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Producto)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false)
                    .HasColumnName("producto")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Totalpropuesta)
                    .HasColumnType("numeric(20, 4)")
                    .HasColumnName("TOTALPROPUESTA");

                entity.Property(e => e.Totalpropuestausd)
                    .HasColumnType("numeric(20, 4)")
                    .HasColumnName("totalpropuestausd");

                entity.Property(e => e.Vendedor)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("vendedor")
                    .UseCollation("Modern_Spanish_CI_AS");
            });

            modelBuilder.Entity<VRetencione>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_Retenciones");

                entity.Property(e => e.FechaFactura)
                    .HasColumnType("datetime")
                    .HasColumnName("fecha_factura");

                entity.Property(e => e.FechaRecuperacion)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha Recuperacion");

                entity.Property(e => e.FechaRetencion)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha Retencion");

                entity.Property(e => e.IdCliente)
                    .IsRequired()
                    .HasMaxLength(10)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Monto).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Porcentaje).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<VRetencionesCobrada>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_RetencionesCobradas");

                entity.Property(e => e.BsEmision).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Diferencia).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DocumentoSap)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DolaresComprobante).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DolaresContabilizacion).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DolaresFechaFactura).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FechaComprobante).HasColumnType("datetime");

                entity.Property(e => e.FechaContabilizacion).HasColumnType("datetime");

                entity.Property(e => e.FechaFactura).HasColumnType("datetime");

                entity.Property(e => e.NombreCliente)
                    .HasMaxLength(200)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NroComprobante)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Oficina)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Porcentaje).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.TasaComprobante).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TasaContabilizacion).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Vendedor)
                    .HasMaxLength(100)
                    .UseCollation("Modern_Spanish_CI_AS");
            });

            modelBuilder.Entity<VSolicitudesExcepcionPendiente>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_SolicitudesExcepcionPendientes");

                entity.Property(e => e.Cotizacion)
                    .HasMaxLength(13)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.FechaAprobacion).HasColumnType("datetime");

                entity.Property(e => e.FechaCompromiso).HasColumnType("datetime");

                entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

                entity.Property(e => e.FechaRechazo).HasColumnType("datetime");

                entity.Property(e => e.IdCliente)
                    .HasMaxLength(10)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.IdOficina)
                    .HasMaxLength(10)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.IdRegion)
                    .HasMaxLength(10)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.IdVendedor)
                    .HasMaxLength(4)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.MontoPagado).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.NombreCliente)
                    .HasMaxLength(80)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreOficina)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreVendedor)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.ObservacionesAprobador).UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.ObservacionesCredito).UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Orden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.SolicitudDeCredito).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.UsdCotizado).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.UsuarioAprobador)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.UsuarioCreacion)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.UsuarioRechazo)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");
            });

            modelBuilder.Entity<VTemporalComisione>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_Temporal_Comisiones");

                entity.Property(e => e.BaseComsion)
                    .HasColumnType("numeric(38, 2)")
                    .HasColumnName("Base Comsion");

                entity.Property(e => e.BsComision).HasColumnType("numeric(38, 2)");

                entity.Property(e => e.Cliente)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.ComCuotasCump).HasColumnType("numeric(38, 2)");

                entity.Property(e => e.ComCuotasGenerales)
                    .HasColumnType("numeric(38, 2)")
                    .HasColumnName("ComCuotas Generales");

                entity.Property(e => e.Concesion).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DescripcionTipoPago)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.FechaOrden).HasColumnType("datetime");

                entity.Property(e => e.Vendedor)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");
            });

            modelBuilder.Entity<VTemporalComsione>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_TemporalComsiones");

                entity.Property(e => e.BsBaseComsion).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.BsComision).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.CantidadOrden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.ComCuotasCump).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ComCuotasGenerales)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("ComCuotas Generales");

                entity.Property(e => e.Concepto).HasMaxLength(100);

                entity.Property(e => e.Concesion).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Descripcion).HasMaxLength(255);

                entity.Property(e => e.DescripcionTipoPago).HasMaxLength(255);

                entity.Property(e => e.FechaOrden).HasColumnType("datetime");

                entity.Property(e => e.FechaRegistro).HasColumnType("datetime");

                entity.Property(e => e.FechaTasa).HasColumnType("datetime");

                entity.Property(e => e.IdCliente).HasMaxLength(10);

                entity.Property(e => e.IdVendedor).HasMaxLength(50);

                entity.Property(e => e.LineaNegocio).HasMaxLength(255);

                entity.Property(e => e.MontoRecibo).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.NombreCliente).HasMaxLength(255);

                entity.Property(e => e.NombreOficina).HasMaxLength(255);

                entity.Property(e => e.NombreVendedor).HasMaxLength(255);

                entity.Property(e => e.Orden).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.Porc)
                    .HasColumnType("decimal(38, 20)")
                    .HasColumnName("porc");

                entity.Property(e => e.PorcComisiones).HasColumnType("numeric(18, 4)");

                entity.Property(e => e.Porcentaje).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Producto).HasMaxLength(12);

                entity.Property(e => e.Sobreprecio).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Subcategoria).HasMaxLength(255);

                entity.Property(e => e.Tasa).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UsdBaseComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComision).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionCantidadCuotasCumplidas).HasColumnType("decimal(20, 2)");

                entity.Property(e => e.UsdComisionRangoCumplimientoCuotaGeneral).HasColumnType("decimal(20, 2)");
            });

            modelBuilder.Entity<VTiemposActualizacionEstadoCuentaMultimonedum>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_TiemposActualizacionEstadoCuentaMultimoneda");

                entity.Property(e => e.FinEjecucion).HasColumnType("datetime");

                entity.Property(e => e.InicioEjecucion).HasColumnType("datetime");
            });

            modelBuilder.Entity<VTiemposWepy208>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("v_TiemposWepy208");

                entity.Property(e => e.Anchos).HasColumnName("ANCHOS");

                entity.Property(e => e.Basica).HasColumnName("BASICA");

                entity.Property(e => e.CantOrdenada).HasColumnName("CANT_ORDENADA");

                entity.Property(e => e.CantidadRodada).HasColumnName("CANTIDAD_RODADA");

                entity.Property(e => e.Cliente).HasColumnName("CLIENTE");

                entity.Property(e => e.CodigoTiempo).HasColumnName("CODIGO_TIEMPO");

                entity.Property(e => e.DescCosto)
                    .IsRequired()
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("DESC_COSTO")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DescMaquina)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .HasColumnName("DESC_MAQUINA")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DescProducto)
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("DESC_PRODUCTO")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DescTiempo)
                    .IsRequired()
                    .HasMaxLength(60)
                    .IsUnicode(false)
                    .HasColumnName("DESC_TIEMPO")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Fecha)
                    .HasColumnType("datetime")
                    .HasColumnName("FECHA");

                entity.Property(e => e.FormasRodada).HasColumnName("FORMAS_RODADA");

                entity.Property(e => e.LineaProducto)
                    .IsRequired()
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .HasColumnName("LINEA_PRODUCTO")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Maquina).HasColumnName("MAQUINA");

                entity.Property(e => e.NombreCliente)
                    .IsRequired()
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_CLIENTE")
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreOperador)
                    .HasMaxLength(35)
                    .IsUnicode(false)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Operador).HasColumnName("OPERADOR");

                entity.Property(e => e.Opuesta).HasColumnName("OPUESTA");

                entity.Property(e => e.Orden).HasColumnName("ORDEN");

                entity.Property(e => e.PiesRodados).HasColumnName("PIES_RODADOS");

                entity.Property(e => e.PiesTotales).HasColumnName("PIES_TOTALES");

                entity.Property(e => e.Producto)
                    .HasMaxLength(12)
                    .HasColumnName("PRODUCTO")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Tiempo)
                    .HasColumnType("decimal(38, 2)")
                    .HasColumnName("TIEMPO");

                entity.Property(e => e.TotalPropuestaUsd).HasColumnType("numeric(20, 4)");

                entity.Property(e => e.VentaRodado).HasColumnName("VENTA_RODADO");
            });

            modelBuilder.Entity<VVenta>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_Ventas");

                entity.Property(e => e.Alo)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("ALO");

                entity.Property(e => e.AloContable)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("ALO_CONTABLE");

                entity.Property(e => e.Bsmc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BSMC");

                entity.Property(e => e.Categoria).HasMaxLength(50);

                entity.Property(e => e.ClientePadre)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CodJdePadre).HasColumnName("Cod_jdePadre");

                entity.Property(e => e.CodOficina).HasColumnName("COD_OFICINA");

                entity.Property(e => e.CodigoCliente)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Codigo_cliente")
                    .IsFixedLength();

                entity.Property(e => e.Codmos)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CODMOS")
                    .IsFixedLength();

                entity.Property(e => e.Conteo)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("CONTEO");

                entity.Property(e => e.Decuentousd)
                    .HasColumnType("decimal(38, 6)")
                    .HasColumnName("DECUENTOUSD");

                entity.Property(e => e.Descripcion).HasMaxLength(50);

                entity.Property(e => e.DescripcionLineaNegocio).HasMaxLength(100);

                entity.Property(e => e.DescripcionVariacion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Descuento)
                    .HasColumnType("decimal(38, 6)")
                    .HasColumnName("DESCUENTO");

                entity.Property(e => e.EstatusCliente)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FechaContable).HasColumnType("datetime");

                entity.Property(e => e.FechaDolRef)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_dol_ref");

                entity.Property(e => e.FechaOrden)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_Orden");

                entity.Property(e => e.Fiscal)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.IdSolicitud).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.KilosOrden).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LineaProduc)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("LINEA_PRODUC")
                    .IsFixedLength();

                entity.Property(e => e.Mes)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("MES");

                entity.Property(e => e.MesContable)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("MES_CONTABLE");

                entity.Property(e => e.NomOficina)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOM_OFICINA");

                entity.Property(e => e.NomVendedor)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("NOM_VENDEDOR")
                    .IsFixedLength();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE")
                    .IsFixedLength();

                entity.Property(e => e.NombreRamo)
                    .HasMaxLength(200)
                    .HasColumnName("NOMBRE_RAMO");

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION");

                entity.Property(e => e.NombreSector)
                    .HasMaxLength(200)
                    .HasColumnName("NOMBRE_SECTOR");

                entity.Property(e => e.Orden)
                    .HasColumnType("decimal(10, 0)")
                    .HasColumnName("ORDEN");

                entity.Property(e => e.OrdenEstrategica).HasMaxLength(2);

                entity.Property(e => e.PiesCuadrado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PorcVentasMaximo).HasColumnType("numeric(38, 6)");

                entity.Property(e => e.PorcVentasMinimo).HasColumnType("numeric(38, 6)");

                entity.Property(e => e.Porcentajedescuento)
                    .HasMaxLength(10)
                    .HasColumnName("PORCENTAJEDESCUENTO")
                    .IsFixedLength();

                entity.Property(e => e.Pormc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("PORMC");

                entity.Property(e => e.PrecioMaximo)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("precio_maximo");

                entity.Property(e => e.PrecioMinimo).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.PrecioVenta)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("PRECIO_VENTA");

                entity.Property(e => e.PrecioVentareconvertido)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("PRECIO_VENTAReconvertido");

                entity.Property(e => e.Producto)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCTO")
                    .IsFixedLength();

                entity.Property(e => e.Region)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("REGION");

                entity.Property(e => e.RegionJunta)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("Region_Junta");

                entity.Property(e => e.SubCategoria).HasMaxLength(50);

                entity.Property(e => e.Subcategoria1)
                    .HasMaxLength(50)
                    .HasColumnName("subcategoria1");

                entity.Property(e => e.TasaDolRef)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("Tasa_Dol_Ref");

                entity.Property(e => e.TasaParalelo).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.TasaPromedio).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Tipo)
                    .HasMaxLength(15)
                    .HasColumnName("TIPO");

                entity.Property(e => e.TipoCalculo)
                    .HasMaxLength(9)
                    .IsUnicode(false)
                    .HasColumnName("Tipo_Calculo");

                entity.Property(e => e.UsdParalelo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UsdPromedio).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Usdmc)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("USDMC");

                entity.Property(e => e.ValorVenta)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("VALOR_VENTA");

                entity.Property(e => e.ValorVentareconvertido)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("VALOR_VENTAReconvertido");

                entity.Property(e => e.Vendedor)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("VENDEDOR")
                    .IsFixedLength();

                entity.Property(e => e.VentaDolRef)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Venta_Dol_Ref");
            });
            
               modelBuilder.Entity<Ventas>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("Ventas");

            

                entity.Property(e => e.AloContable)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("ALO_CONTABLE");

             

                entity.Property(e => e.Categoria).HasMaxLength(50);

               

                entity.Property(e => e.CodOficina).HasColumnName("COD_OFICINA");

         


            

          

                entity.Property(e => e.MesContable)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("MES_CONTABLE");

                entity.Property(e => e.NomOficina)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOM_OFICINA");

                entity.Property(e => e.NomVendedor)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("NOM_VENDEDOR")
                    .IsFixedLength();

                entity.Property(e => e.Cliente)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("CLIENTE")
                    .IsFixedLength();

              

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION");

              

              

                entity.Property(e => e.Region)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("REGION");

             

                entity.Property(e => e.SubCategoria).HasMaxLength(50);

              

               

              

                entity.Property(e => e.Vendedor)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("VENDEDOR")
                    .IsFixedLength();

                entity.Property(e => e.VentaDolares)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Venta_Dolares");
            });

            modelBuilder.Entity<VVtascuota>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("V_VTASCUOTAS");

                entity.Property(e => e.Budget).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Clientes).HasColumnName("clientes");

                entity.Property(e => e.CodigoRegion)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CODIGO_REGION")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Conteo)
                    .HasColumnType("decimal(38, 0)")
                    .HasColumnName("CONTEO");

                entity.Property(e => e.CuotaUsd).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.DescripcionCuota)
                    .HasMaxLength(50)
                    .IsFixedLength()
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.DescripcionLineaNegocio)
                    .HasMaxLength(50)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Kilosorden)
                    .HasColumnType("decimal(38, 2)")
                    .HasColumnName("KILOSORDEN");

                entity.Property(e => e.NomOficina)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOM_OFICINA")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreRegion)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.NombreVendedor)
                    .HasMaxLength(30)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.Piescuadrado).HasColumnType("decimal(38, 2)");

                entity.Property(e => e.RegionJunta)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("Region_Junta")
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.UsdParalelo).HasColumnType("decimal(38, 2)");

                entity.Property(e => e.UsdPromedio).HasColumnType("decimal(38, 2)");

                entity.Property(e => e.UsdVentasEstrategica).HasColumnType("decimal(38, 2)");

                entity.Property(e => e.Vendedor)
                    .HasMaxLength(4)
                    .UseCollation("Modern_Spanish_CI_AS");

                entity.Property(e => e.VentaUsd).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.VentaUsdFormato).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<Vventa1>(entity =>
            {
                entity.ToTable("VVentas");

                entity.Property(e => e.Id)
                    .HasColumnType("numeric(18, 0)")
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ID");

                entity.Property(e => e.Alo)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("ALO");

                entity.Property(e => e.AloContable)
                    .HasColumnType("decimal(4, 0)")
                    .HasColumnName("ALO_CONTABLE");

                entity.Property(e => e.Bsmc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("BSMC");

                entity.Property(e => e.Categoria).HasMaxLength(50);

                entity.Property(e => e.ClientePadre)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.CodJdePadre).HasColumnName("Cod_jdePadre");

                entity.Property(e => e.CodOficina).HasColumnName("COD_OFICINA");

                entity.Property(e => e.CodigoCliente)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("Codigo_cliente")
                    .IsFixedLength();

                entity.Property(e => e.Codmos)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("CODMOS")
                    .IsFixedLength();

                entity.Property(e => e.Conteo)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("CONTEO");

                entity.Property(e => e.Decuentousd)
                    .HasColumnType("decimal(38, 6)")
                    .HasColumnName("DECUENTOUSD");

                entity.Property(e => e.Descripcion).HasMaxLength(50);

                entity.Property(e => e.DescripcionLineaNegocio).HasMaxLength(100);

                entity.Property(e => e.DescripcionVariacion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Descuento)
                    .HasColumnType("decimal(38, 6)")
                    .HasColumnName("DESCUENTO");

                entity.Property(e => e.EstatusCliente)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.FechaContable).HasColumnType("datetime");

                entity.Property(e => e.FechaDolRef)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_dol_ref");

                entity.Property(e => e.FechaOrden)
                    .HasColumnType("datetime")
                    .HasColumnName("Fecha_Orden");

                entity.Property(e => e.Fiscal)
                    .HasMaxLength(9)
                    .IsUnicode(false);

                entity.Property(e => e.IdSolicitud).HasColumnType("numeric(18, 0)");

                entity.Property(e => e.KilosOrden).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.LineaProduc)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("LINEA_PRODUC")
                    .IsFixedLength();

                entity.Property(e => e.Mes)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("MES");

                entity.Property(e => e.MesContable)
                    .HasColumnType("decimal(2, 0)")
                    .HasColumnName("MES_CONTABLE");

                entity.Property(e => e.NomOficina)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOM_OFICINA");

                entity.Property(e => e.NomVendedor)
                    .HasMaxLength(30)
                    .IsUnicode(false)
                    .HasColumnName("NOM_VENDEDOR")
                    .IsFixedLength();

                entity.Property(e => e.Nombre)
                    .HasMaxLength(80)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE")
                    .IsFixedLength();

                entity.Property(e => e.NombreRamo)
                    .HasMaxLength(200)
                    .HasColumnName("NOMBRE_RAMO");

                entity.Property(e => e.NombreRegion)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("NOMBRE_REGION");

                entity.Property(e => e.NombreSector)
                    .HasMaxLength(200)
                    .HasColumnName("NOMBRE_SECTOR");

                entity.Property(e => e.Orden)
                    .HasColumnType("decimal(10, 0)")
                    .HasColumnName("ORDEN");

                entity.Property(e => e.OrdenEstrategica).HasMaxLength(2);

                entity.Property(e => e.PiesCuadrado).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PorcVentasMaximo).HasColumnType("numeric(38, 6)");

                entity.Property(e => e.PorcVentasMinimo).HasColumnType("numeric(38, 6)");

                entity.Property(e => e.Porcentajedescuento)
                    .HasMaxLength(10)
                    .HasColumnName("PORCENTAJEDESCUENTO")
                    .IsFixedLength();

                entity.Property(e => e.Pormc)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("PORMC");

                entity.Property(e => e.PrecioMaximo)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("precio_maximo");

                entity.Property(e => e.PrecioMinimo).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.PrecioVenta)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("PRECIO_VENTA");

                entity.Property(e => e.PrecioVentareconvertido)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("PRECIO_VENTAReconvertido");

                entity.Property(e => e.Producto)
                    .HasMaxLength(12)
                    .IsUnicode(false)
                    .HasColumnName("PRODUCTO")
                    .IsFixedLength();

                entity.Property(e => e.Region)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .HasColumnName("REGION");

                entity.Property(e => e.RegionJunta)
                    .HasMaxLength(25)
                    .IsUnicode(false)
                    .HasColumnName("Region_Junta");

                entity.Property(e => e.SubCategoria).HasMaxLength(50);

                entity.Property(e => e.Subcategoria1)
                    .HasMaxLength(50)
                    .HasColumnName("subcategoria1");

                entity.Property(e => e.TasaDolRef)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("Tasa_Dol_Ref");

                entity.Property(e => e.TasaParalelo).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.TasaPromedio).HasColumnType("decimal(16, 2)");

                entity.Property(e => e.Tipo)
                    .HasMaxLength(15)
                    .HasColumnName("TIPO");

                entity.Property(e => e.TipoCalculo)
                    .HasMaxLength(9)
                    .IsUnicode(false)
                    .HasColumnName("Tipo_Calculo");

                entity.Property(e => e.UsdParalelo).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.UsdPromedio).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Usdmc)
                    .HasColumnType("numeric(18, 2)")
                    .HasColumnName("USDMC");

                entity.Property(e => e.ValorVenta)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("VALOR_VENTA");

                entity.Property(e => e.ValorVentareconvertido)
                    .HasColumnType("decimal(16, 2)")
                    .HasColumnName("VALOR_VENTAReconvertido");

                entity.Property(e => e.Vendedor)
                    .HasMaxLength(4)
                    .IsUnicode(false)
                    .HasColumnName("VENDEDOR")
                    .IsFixedLength();

                entity.Property(e => e.VentaDolRef)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("Venta_Dol_Ref");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
