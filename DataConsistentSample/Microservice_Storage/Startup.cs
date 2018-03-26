using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microservice_Storage.EventHandlers;
using Microservice_Storage.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microservice_Storage
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
            services.AddSingleton<IStorageRepository, StorageRepository>();
            services.AddMassTransit(c =>
            {
                c.AddConsumer<StoreageOrderEventHandler>();
            });
            services.AddScoped<StoreageOrderEventHandler>();
            services.AddMvc();
        }

        public static IBusControl BusControl { get; private set; }
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            BusControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost"), hst =>
                {
                    hst.Username("guest");
                    hst.Password("guest");
                });

                cfg.ReceiveEndpoint(host, "storeage_order", e =>
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
