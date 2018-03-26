using IntegrationEvents;
using MassTransit;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_BackgroundTask.Model
{
    public class OrderEventJob : IJob
    {
        IEventRepository _eventRepository;
        IBusControl _bus;
        public OrderEventJob(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
            _bus =Startup.BusControl;
        }
     
        public Task Execute(IJobExecutionContext context)
        {
            var events = _eventRepository.GetEvent().GetAwaiter().GetResult();
            foreach (var eventItem in events)
            {
                IOrder order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(eventItem.EntityJson);
                _bus.Publish(order);
            }

            return Task.CompletedTask;
        }

    }
}
