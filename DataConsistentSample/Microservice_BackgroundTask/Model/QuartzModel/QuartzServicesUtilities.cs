
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_BackgroundTask.Model
{
    public static class QuartzServicesUtilities
    {
        public async static void StartJob<TJob>(IScheduler scheduler)
            where TJob : IJob
        {
            var jobName = typeof(TJob).FullName;

            var job = JobBuilder.Create<TJob>()
                .WithIdentity(jobName)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobName}.trigger")
                .StartNow()
                .WithSimpleSchedule(scheduleBuilder =>
                    scheduleBuilder
                        .WithInterval(TimeSpan.FromSeconds(3))
                        .RepeatForever())
                .Build();



            await scheduler.ScheduleJob(job, trigger);
        }
    }
}
