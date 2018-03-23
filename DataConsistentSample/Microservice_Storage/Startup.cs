using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
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
            StartESB(services);
            services.AddMvc();
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }

        void StartESB(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var storageRepository = provider.GetService<IStorageRepository>();
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>            {
                var host = cfg.Host(new Uri("rabbitmq://localhost"), hst =>
                {
                    hst.Username("guest");
                    hst.Password("guest");
                });
                cfg.ReceiveEndpoint(host, "storeage_order", e =>
                {
                    e.Consumer(() => new StoreageOrderEventHandler(storageRepository));
                });
            });
            bus.Start();
            services.AddSingleton(bus);

        }
    }
}
