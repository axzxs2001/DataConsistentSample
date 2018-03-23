using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microservice_BackgroundTask.Model;
using Microservice_Order.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;


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
            // services.AddSingleton<OrderEventJob>();
            StartESB(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            StartTestAsync(app).GetAwaiter().GetResult();
        }

        public static async Task StartTestAsync(IApplicationBuilder app)
        {
            try
            {
                OrderEventJob._eventRepository = app.ApplicationServices.GetService<IEventRepository>();
                OrderEventJob._bus= app.ApplicationServices.GetService<IBusControl>();
                //从工厂中获取调度程序实例
                var props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
                var factory = new StdSchedulerFactory(props);
                var scheduler = await factory.GetScheduler();

                //开启调度器
                await scheduler.Start();

                //定义这个工作，并将其绑定到我们的IJob实现类
                var job = JobBuilder.Create<OrderEventJob>()

                    .WithIdentity("OrderEventJob", "OrderEventGroup")
                    .Build();

                //触发作业立即运行，然后每10秒重复一次，无限循环
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("OrderEventTrigger", "OrderEventGroup")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(30)
                        .RepeatForever())
                    .Build();
                //告诉Quartz使用我们的触发器来安排作业
                await scheduler.ScheduleJob(job, trigger);
            }
            catch (SchedulerException se)
            {
                await Console.Error.WriteLineAsync(se.ToString());
            }
        }

        void StartESB(IServiceCollection services)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                var host = cfg.Host(new Uri("rabbitmq://localhost"), hst =>
                {
                    hst.Username("guest");
                    hst.Password("guest");
                });
            });
            services.AddSingleton(bus);
        }
    }
}
