using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microservice_BackgroundTask.Model;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Microservice_BackgroundTask;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microservice_BackgroundTask.EventHandlers;
using GreenPipes;

namespace Microservice_BackgroundTask
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
            services.AddTransient<IEventRepository, EventRepository>();

            services.UseQuartz(typeof(OrderEventJob));
            services.AddMassTransit(c =>
            {
                c.AddConsumer<OrderEventHandler>();
            });
            services.AddScoped<OrderEventHandler>();
            services.AddMvc();
        }

        public static IBusControl BusControl { get; private set; }
        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider, IHostingEnvironment env, IApplicationLifetime applicationLifetime, IScheduler scheduler)
        {
            BusControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
               
                //重试
                cfg.UseRetry(ret =>
                {
                    //每隔10秒试一闪，共试3次
                    ret.Interval(6, TimeSpan.FromSeconds(10));
                });
                //限流 100秒内限1000千次请求
                cfg.UseRateLimit(1000, TimeSpan.FromSeconds(100));
                //熔断
                cfg.UseCircuitBreaker(cb =>
                {
                    //跟踪周期
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    //成功/失败的比例，15%，会打开熔断器
                    cb.TripThreshold = 15;
                    //至少请求数字，少于10不启用熔断器
                    cb.ActiveThreshold = 5;
                    //断路后与再次偿试的时间间隔
                    cb.ResetInterval= TimeSpan.FromMinutes(5);

                });
                cfg.ReceiveEndpoint(host, "order_event", e =>
                {                    
                    e.LoadFrom(serviceProvider);
                });
            });

            applicationLifetime.ApplicationStarted.Register(BusControl.Start);
            applicationLifetime.ApplicationStopped.Register(BusControl.Stop);

            //每隔30称执行一次OrderEventJob的Execute方法
            QuartzServicesUtilities.StartJob<OrderEventJob>(scheduler, TimeSpan.FromSeconds(30));
            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }



    }



}
