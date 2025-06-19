using AppService.Core.CustomEntities;
using AppService.Core.Interfaces;
using AppService.Core.Services;
using AppService.Infrastructure.Data;
using AppService.Infrastructure.DataClientes;
using AppService.Infrastructure.DataContratosStock;
using AppService.Infrastructure.DataCXC;
using AppService.Infrastructure.DataDW;
using AppService.Infrastructure.DataEstadisticas;
using AppService.Infrastructure.DataFacturacion;
using AppService.Infrastructure.DataMaestros;
using AppService.Infrastructure.DataMateriales;
using AppService.Infrastructure.DataMc;

using AppService.Infrastructure.DataMooreve;
using AppService.Infrastructure.DataNomina;
using AppService.Infrastructure.DataPlanta;
using AppService.Infrastructure.DataSap;
using AppService.Infrastructure.DataSpi;
using AppService.Infrastructure.Filters;
using AppService.Infrastructure.Interfaces;
using AppService.Infrastructure.Repositories;
using AppService.Infrastructure.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using AppService.Core.Interfaces.Comisiones;
using AppService.Core.Interfaces.Cotizaciones;
using AppService.Core.Interfaces.PowerBi;
using AppService.Core.Interfaces.Ventas;
using AppService.Core.Services.PowerBi;
using AppService.Infrastructure.DataPowerBI;
using StackExchange.Redis;

namespace AppService.Api
{
    public class Startup
    {


        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddCors();




            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));




            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            }).ConfigureApiBehaviorOptions(options =>
            {
                // options.SuppressModelStateInvalidFilter = true;
            });

            services.Configure<PaginationOptions>(Configuration.GetSection("Pagination"));

            services.AddDbContext<RRDContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("rrdConecction"),
                    sqlServerOptions => sqlServerOptions.CommandTimeout(3600)
                   );

            });

            services.AddDbContext<SapContext>(options =>

               options.UseSqlServer(Configuration.GetConnectionString("sapConecction"),
                   sqlServerOptions => sqlServerOptions.CommandTimeout(3600))

                );

            /*services.AddDbContext<MooreveContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("mooreveConecction")); 

            });*/


             services.AddDbContext<MooreveContext>(
                 x => x.UseSqlServer(Configuration.GetConnectionString("mooreveConecction"),
                     sqlServerOptions => sqlServerOptions.CommandTimeout(3600)
                 ),
                 ServiceLifetime.Transient);
             
             
             services.AddDbContext<POWERBIContext>(
                 x => x.UseSqlServer(Configuration.GetConnectionString("powerbiConecction"),
                     sqlServerOptions => sqlServerOptions.CommandTimeout(3600)
                 ),
                 ServiceLifetime.Transient);

             

            services.AddDbContext<MaterialesContext>(options =>

              options.UseSqlServer(Configuration.GetConnectionString("materialesConecction"),
                  sqlServerOptions => sqlServerOptions.CommandTimeout(3600))

               );

            services.AddDbContext<ClientesContext>(options =>

             options.UseSqlServer(Configuration.GetConnectionString("clientesConecction"),
                 sqlServerOptions => sqlServerOptions.CommandTimeout(3600))

              );


            services.AddDbContext<MCContext>(options =>

            options.UseSqlServer(Configuration.GetConnectionString("mcConecction"),
                sqlServerOptions => sqlServerOptions.CommandTimeout(3600))

             );

            services.AddDbContext<CXCContext>(options =>

                options.UseSqlServer(Configuration.GetConnectionString("cxcConecction"))

            );
            services.AddDbContext<IMaestrosContext>(options =>

               options.UseSqlServer(Configuration.GetConnectionString("maestrosConecction"),
                   sqlServerOptions => sqlServerOptions.CommandTimeout(3600))

           );

            services.AddDbContext<FacturacionContext>(options =>

                          options.UseSqlServer(Configuration.GetConnectionString("facturacionConecction"),
                          sqlServerOptions => sqlServerOptions.CommandTimeout(3600)
)

                      );

          

            services.AddDbContext<PlantaContext>(options =>

                         options.UseSqlServer(Configuration.GetConnectionString("plantaConecction"),
                             sqlServerOptions => sqlServerOptions.CommandTimeout(3600))

                     );



            services.AddDbContext<ContratosStockContext>(options =>

              options.UseSqlServer(Configuration.GetConnectionString("contratosStockConecction"),
                  sqlServerOptions => sqlServerOptions.CommandTimeout(3600))

          );
            services.AddDbContext<DWContext>(options =>

              options.UseSqlServer(Configuration.GetConnectionString("dwConecction"),
                  sqlServerOptions => sqlServerOptions.CommandTimeout(3600))

          );

            services.AddDbContext<NominaContext>(options =>

               options.UseSqlServer(Configuration.GetConnectionString("nominaConecction"),
                   sqlServerOptions => sqlServerOptions.CommandTimeout(3600))
          );
            services.AddDbContext<EstadisticasContext>(options =>

           options.UseSqlServer(Configuration.GetConnectionString("estadisticasConecction"),
               sqlServerOptions => sqlServerOptions.CommandTimeout(3600))
            );

            services.AddDbContext<SpiContext>(options =>
                      options.UseOracle(Configuration.GetConnectionString("spiConnection"), b => b.UseOracleSQLCompatibility("11")).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(Configuration.GetConnectionString("redisConnection")));

            
            
            services.AddTransient<IPowerBiOrdenesService, PowerBiOrdenesService>();

           
            services.AddTransient<IAppOrdenProductoRepeticionServices, AppOrdenProductoRepeticionServices>();
            services.AddTransient<ICobAdjuntosCobranzaService, CobAdjuntosCobranzaService>();
            services.AddTransient<IAppAdjuntosCotizacionService, AppAdjuntosCotizacionService>();
            
            services.AddTransient<IGeneralCobranzaService, GeneralCobranzaService>();
            services.AddTransient<ISegUsuarioService, SegUsuarioService>();
            services.AddTransient<IMtrClienteService, MtrClienteService>();
            services.AddTransient<ICobTipoTransaccionService, CobTipoTransaccionService>();
            services.AddTransient<IOfdTipoDocumentoService, OfdTipoDocumentoService>();
            services.AddTransient<IMtrVendedorService, MtrVendedorService>();
            services.AddTransient<IMtrOficinaService, MtrOficinaService>();
            services.AddTransient<IMtrBancosService, MtrBancosService>();
            services.AddTransient<IMtrTipoMonedaService, MtrTipoMonedaService>();
            services.AddTransient<ICobGrabacionCobranzasService, CobGrabacionCobranzasService>();
            services.AddTransient<IMtrTipoTransaccionCuentaDestinoService, MtrTipoTransaccionCuentaDestinoService>();
            services.AddTransient<ICobTransaccionesService, CobTransaccionesService>();
            services.AddTransient<ICobEstadoCuentaService, CobEstadoCuentaService>();
            services.AddTransient<IOfdCotizacionService, OfdCotizacionService>();
            services.AddTransient<ICobPagosRetencionesService, CobPagosRetencionesService>();
            services.AddTransient<ICobSysfileService, CobSysfileService>();
            services.AddTransient<ISapRetencionesService, SapRetencionesService>();
            services.AddTransient<ICobValoresRetencionesService, CobValoresRetencionesService>();
            services.AddTransient<IMtrContactosService, MtrContactosService>();
            services.AddTransient<ITPaTasaBancoCentralServices, TPaTasaBancoCentralServices>();
            services.AddTransient<ICobMatrixMonedaValidaPagoService, CobMatrixMonedaValidaPagoService>();

            services.AddTransient<IAppGeneralQuotesService, AppGeneralQuotesService>();

            services.AddTransient<IMtrCondicionPagoService, MtrCondicionPagoService>();
            services.AddTransient<IAppStatusQuoteService, AppStatusQuoteService>();
            services.AddTransient<IAppUnitsService, AppUnitsService>();
            services.AddTransient<IAppSubCategoryService, AppSubCategoryService>();
            services.AddTransient<IAppProductsService, AppProductsService>();
            services.AddTransient<IAppDetailQuotesService, AppDetailQuotesService>();
            services.AddTransient<IAppVariablesService, AppVariablesService>();
            services.AddTransient<IAppIngredientsService, AppIngredientsService>();
            services.AddTransient<IHelperService, HelperService>();
            services.AddTransient<IAppRecipesServices, AppRecipesServices>();
            services.AddTransient<IAppVariableSearchService, AppVariableSearchService>();
            services.AddTransient<IAppProductVariableSearchTextService, AppProductVariableSearchTextService>();
            services.AddTransient<IAppVariableSearchService, AppVariableSearchService>();
            services.AddTransient<IAppProductVariableSearchTextService, AppProductVariableSearchTextService>();
            services.AddTransient<IAppTemplateConversionUnitService, AppTemplateConversionUnitService>();
            services.AddTransient<IAppDetailQuotesConversionUnitService, AppDetailQuotesConversionUnitService>();
            services.AddTransient<IAppRecipesByAppDetailQuotesService, AppRecipesByAppDetailQuotesService>();
            services.AddTransient<IAppProductConversionService, AppProductConversionService>();
            services.AddTransient<IAppPriceServices, AppPriceServices>();


            services.AddTransient<ITPaTasaReferencialServices, TPaTasaReferencialServices>();
            services.AddTransient<IAppVariablesService, AppVariablesService>();
            services.AddTransient<IAppConfigAppService, AppConfigAppService>();
            services.AddTransient<IAppConfigAppService, AppConfigAppService>();
            services.AddTransient<IAppCostosEmbarqueService, AppCostosEmbarqueService>();

            services.AddTransient<ICompetidoresService, CompetidoresService>();

            services.AddTransient<ICotizacionService, CotizacionService>();

            services.AddTransient<IAprobacionesServices, AprobacionesServices>();

            services.AddTransient<IMunicipioServices, MunicipioServices>();

            services.AddTransient<IAppEspecificacionesServices, AppEspecificacionesServices>();
            services.AddTransient<IPeriodosService, PeriodosService>();
            services.AddTransient<IAppCalculadoraService, AppCalculadoraService>();
            

            services.AddHttpClient<ISapClient, SapClient>();
            services.AddHttpClient<IOdooClient, OdooClient>();
            
            
            services.AddTransient<ITasaConsolidadoService, TasaConsolidadoService>();
            



            services.AddTransient<INmm024Service, Nmm024Service>();
            services.AddTransient<INmt033Service, Nmt033Service>();

            services.AddTransient<IAppDocumentosFiscalesService, AppDocumentosFiscalesService>();
            services.AddTransient<IMotivoGanarPerderService, MotivoGanarPerderService>();

            services.AddTransient<IVCotizacionesPorAprobarService, VCotizacionesPorAprobarService>();
            
            services.AddTransient<IHistoricoComisionesService, HistoricoComisionesService>();

            services.AddTransient<IPagosManualesService, PagosManualesService>();

            services.AddTransient<IAppVentasService, AppVentasService>();
            services.AddTransient<ITipoPagoService, TipoPagoService>();
            services.AddTransient<IPcProcesosService, PcProcesosService>();
            services.AddTransient<IAppQuotesPaginateService, AppQuotesPaginateService>();
            services.AddTransient<IAppGetPriceService, AppGetPriceService>();
            services.AddTransient<IEmaiService, EmaiService>();
            services.AddTransient<IEmailClient, EmailClient>();
            
            
            
            
            services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            //services.AddHostedService<Worker>()
            //        .AddSingleton<ICotizacionService, CotizacionService>();

            services.AddSingleton<IUriService>(provider =>
            {
                IHttpContextAccessor accesor = provider.GetRequiredService<IHttpContextAccessor>();
                HttpRequest request = accesor.HttpContext.Request;
                string absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(absoluteUri);
            });


            services.AddSwaggerGen(doc =>
            {
                doc.SwaggerDoc("v1", new OpenApiInfo { Title = "App Service Api", Version = "v1" });
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                doc.IncludeXmlComments(xmlPath);

            });


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Authentication:Issuer"],
                    ValidAudience = Configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:SecrectKey"])),
                };
            });



            services.AddCors(o => o.AddPolicy("charppolicy", builder =>
            {
                builder

                        .AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            // services.ConfigureHangFire(Configuration);

            services.AddControllersWithViews();


            services.AddMvc(options =>
           {
               options.Filters.Add<ValidationFilter>();
               options.ReturnHttpNotAcceptable = false;
           }).AddFluentValidation(options =>
           {

               options.RegisterValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
           });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
                                IApplicationBuilder app,
                                IWebHostEnvironment env,
                                ILoggerFactory loggerFactory,
                                //IRecurringJobManager recurringJobManager,
                                IServiceProvider serviceProvider)
        {



            loggerFactory.AddFile("Log-{Date}.txt");


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {

                options.SwaggerEndpoint("/swagger/v1/swagger.json", "App Service API V1");
                options.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseCors("charppolicy");

            //  app.UseHangfireDashboard();

            app.UseAuthentication();

            app.UseAuthorization();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            //**GENERAADOR DE EXPRESIONES
            //https://www.freeformatter.com/cron-expression-generator-quartz.html
            //**

            //var cotizacionServices = serviceProvider.GetService<ICotizacionService>();
            // recurringJobManager.AddOrUpdate("Integrar_cotizaciones_odoo", () => cotizacionServices.UpdateCotizacionesToOdoo(), "*/30 * * * * *");



            // var cobEstadoCuentaService = serviceProvider.GetService<ICobEstadoCuentaService>();
            // recurringJobManager.AddOrUpdate("ActualizarEstadoCuentaMultimonedas", () => cobEstadoCuentaService.GenerateEstadoCuentaMultimoneda(), Cron.Daily());


        }
    }
}
