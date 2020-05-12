using System;
using Configuration;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using DataProvider.Infrastructure.Extantions;
using NotaryProvider.Services;
using NotaryProvider.HostedServices;

namespace NotaryProvider
{
    public class Startup
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SetNlogConfig();
            Logger.AddLogStart("! ! !   --> NotaryProvider <-- START   ! ! !");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Sophia.NotaryProvider";
                    document.Info.Description = "SOPHIA.NOTARYPROVIDER";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Volodymyr Dmyterko",
                        Email = "v.dmyterko@bankvostok.com.ua"
                    };
                };
            });

            services.AddMassTransit(config =>
            {
                config.AddBus(ConfigureBus);
            });
                        
            services.AddHostedService<MassTransitConsumerHostedService>();
            //services.AddHostedService<FileBuilderHostedServiceDI<MassTransitConsumerHostedService>>();
            services.AddHostedService<FileBuilderHostedService>();

            //services.AddTransient<GetDataService>();
            //services.AddSingleton<GetDataService>(new ReturnResponseService());
            services.AddTransient<SavingToDBService>();
            services.AddTransient<IReturnResponseService, ReturnResponseService>(factory => factory.GetService<ReturnResponseService>());
            services.AddTransient<ReturnResponseService>();
            services.AddSingleton<IFileComposeService>(new FileComposeService());
            services.AddSingleton<FileComposeService>();
            //services.AddSingleton(new GetDataService(new ReturnResponseService()));
            services.AddSingleton<IGetDataService>(new GetDataService());
            services.AddSingleton<GetDataService>();
            services.AddTransient<GetDataService>((dsvc) =>
            {
                IReturnResponseService svc = dsvc.GetService<IReturnResponseService>();
                return new GetDataService(svc);
            });
        }

        private void SetNlogConfig()
        {
            NLog.GlobalDiagnosticsContext.Set("DbConnectionString", AppSettings.DbConnectionString);
            //NLog.GlobalDiagnosticsContext.Set("DbConnectionStringPassword", AppSettings.DbConnectionStringPassword);
            NLog.LogManager.LoadConfiguration("NLog.config");
        }

        private IBusControl ConfigureBus(IServiceProvider serviceProvider)
        {
            IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                var host = config.Host(ConfigRabbit.HostAddress, cfg =>
                {
                    cfg.Password(ConfigRabbit.Password);
                    cfg.Username(ConfigRabbit.Username);
                });
                config.UseMessageRetry(msgConfig =>
                {
                    msgConfig.Interval(100, 1000 * 60);
                });
                config.ReceiveEndpoint("SophiaUploader", cfg =>
                {
                    //cfg.ConfigureConsumer<FilePartConsumer>(serviceProvider);
                    cfg.Consumer<FilePartConsumer>();
                });
                config.ReceiveEndpoint("SophiaFrontApi", cfg =>
                {
                    //cfg.ConfigureConsumer<RequestConsumer>(serviceProvider);
                    cfg.Consumer<RequestConsumer>();
                });
                // Для відпарвки ReceiveEndpoint непотрібний!!!
            });

            return busControl;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOpenApi();
            app.UseSwaggerUi3();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
