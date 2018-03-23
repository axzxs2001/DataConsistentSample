using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl;

namespace Microservice_BackgroundTask.Controllers
{
    [Route("api/[controller]")]
    public class TestTaskController
    {
    

        public class HelloJob : IJob
        {
            public Task Execute(IJobExecutionContext context)
            {
                Console.Out.WriteLineAsync("Greetings from HelloJob!"+DateTime .Now);
                return Task.CompletedTask;
            }
        }
    }
}

