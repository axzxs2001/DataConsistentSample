using IntegrationEvents;
using MassTransit;
using Microservice_Ship.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_Ship.EventHandlers
{
    public class ShiperOrderEventHandler : IConsumer<IOrder>
    {
        IShipRepository _shipRepository;
        public ShiperOrderEventHandler(IShipRepository shipRepository)
        {           
            _shipRepository = shipRepository;
        }
        public async Task Consume(ConsumeContext<IOrder> context)
        {
            var order = context.Message;
            var result= await _shipRepository.CreateShip(order);
            if (result)
            {
                IOrderEventEntity orderEventEntity = new OrderEventEntity { OrderID = order.ID, EventType = "CreateOrder", ShipStatus = 2 };
                await Startup.BusControl.Publish(orderEventEntity);
            }
        }
    }
}
