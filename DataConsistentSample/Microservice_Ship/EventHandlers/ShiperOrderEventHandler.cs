using IntegrationEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_Ship.EventHandlers
{
    public class ShiperOrderEventHandler : IConsumer<IOrder>
    {
        public async Task Consume(ConsumeContext<IOrder> context)
        {
            await Console.Out.WriteLineAsync($"Microservice_Ship收到订单：{Newtonsoft.Json.JsonConvert.SerializeObject(context.Message)}");
        }
    }
}
