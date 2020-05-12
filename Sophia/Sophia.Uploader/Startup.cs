using Configuration;
using DataProvider.Infrastructure.Extantions;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System;


namespace Sophia.Uploader
{
    public class Startup
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SetNlogConfig();
            Logger.AddLogStart("! ! !   --> Sophia.Uploader <-- START   ! ! !");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        { 
	        services.AddControllers();
            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Sophia.Uploader";
                    document.Info.Description = "SOPHIA.UPLOADER";
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

            services.AddHostedService<SenderProducerHostedService>();
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

                //config.OverrideDefaultBusEndpointQueueName("SophiaUploader");
                config.OverrideDefaultBusEndpointQueueName(ConfigRabbit.RabbitMqQueueName);

                config.UseMessageRetry(msgConfig =>
                {
                    msgConfig.Interval(100, 1000 * 60);
                });

                // У ВІДПРАВНИКУ Endpoint НЕ ПОТРІБНИЙ !!!!
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
