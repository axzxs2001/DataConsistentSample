using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using IntegrationEvents;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.Util;
using Microservice_Ship.EventHandlers;
using Microservice_Ship.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microservice_Ship
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton("server=.;database=OrderDB;uid=sa;pwd=1");
            services.AddTransient<IShipRepository, ShipRepository>();

            services.AddMassTransit(c =>
            {
                c.AddConsumer<ShiperOrderEventHandler>();
            });
            services.AddScoped<ShiperOrderEventHandler>();
            services.AddMvc();
        }
        public static  IBusControl BusControl { get; private set; }
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            BusControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
           {
               var host = cfg.Host(new Uri("rabbitmq://localhost"), hst =>
               {
                   hst.Username("guest");
                   hst.Password("guest");
               });

               cfg.ReceiveEndpoint(host, "shiper_order", e =>
               {
                   e.LoadFrom(serviceProvider);
               });
           });

            applicationLifetime.ApplicationStarted.Register(BusControl.Start);
            applicationLifetime.ApplicationStopped.Register(BusControl.Stop);

            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }

    }

}

