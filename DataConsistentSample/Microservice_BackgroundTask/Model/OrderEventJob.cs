using IntegrationEvents;
using MassTransit;
using Microservice_Order.Model;
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
        public OrderEventJob(IEventRepository eventRepository, IBusControl bus)
        {
            _eventRepository = eventRepository;
            _bus = bus;
        }
     
        public Task Execute(IJobExecutionContext context)
        {
            var events = _eventRepository.GetEvent().GetAwaiter().GetResult();
            foreach (var eventItem in events)
            {
                var order = Newtonsoft.Json.JsonConvert.DeserializeObject<Order>(eventItem.EntityJson);
                _bus.Publish(order);
            }

            return Task.CompletedTask;
        }

    }
}
