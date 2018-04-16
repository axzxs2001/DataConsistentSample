using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using GreenPipes;
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
                   //重试
                   e.UseRetry(ret =>
                   {
                       //每隔10秒试一闪，共试3次
                       ret.Interval(6, TimeSpan.FromSeconds(10));
                   });
                   //限流 100秒内限1000千次请求
                   e.UseRateLimit(1000, TimeSpan.FromSeconds(100));
                   //熔断
                   e.UseCircuitBreaker(cb =>
                   {
                       //跟踪周期
                       cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                       //成功/失败的比例，15%，会打开熔断器
                       cb.TripThreshold = 15;
                       //至少请求数字，少于10不启用熔断器
                       cb.ActiveThreshold = 5;
                       //断路后与再次偿试的时间间隔
                       cb.ResetInterval = TimeSpan.FromMinutes(5);

                   });
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

