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
            await _shipRepository.CreateShip(context.Message);

        }
    }
}
