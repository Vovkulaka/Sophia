using Configuration;
using GreenPipes;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sophia.FrontApi.Services;
using System;

namespace Sophia.FrontApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            SetNlogConfig();
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
                    document.Info.Title = "Sophia.FrontApi";
                    document.Info.Description = "SOPHIA.FRONTAPI";
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
            services.AddSingleton<ResponseComposeService>();
            services.AddSingleton<IResponseComposeService, ResponseComposeService> (factory => factory.GetService<ResponseComposeService>());
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

                config.OverrideDefaultBusEndpointQueueName(ConfigRabbit.RabbitMqQueueName); // SophiaFrontApi

                config.UseMessageRetry(msgConfig =>
                {
                    msgConfig.Interval(100, 1000 * 60);
                });
                config.ReceiveEndpoint("Responce", cfg =>
                {
                    cfg.Consumer<ResponseConsumer>();
                });
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
