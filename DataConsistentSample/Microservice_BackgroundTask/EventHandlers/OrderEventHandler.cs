using IntegrationEvents;
using MassTransit;
using Microservice_BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_BackgroundTask.EventHandlers
{
    public class OrderEventHandler : IConsumer<IOrderEventEntity>
    {
        IEventRepository _eventRepository;
        public OrderEventHandler(IEventRepository  eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task Consume(ConsumeContext<IOrderEventEntity> context)
        {
             var result= _eventRepository.UpdateOrderEnvent(context.Message);
            await result;
        }
    }
}
