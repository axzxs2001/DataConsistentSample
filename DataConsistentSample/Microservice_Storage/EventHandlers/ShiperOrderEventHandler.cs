using IntegrationEvents;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_Storage.EventHandlers
{
    public class StoreageOrderEventHandler : IConsumer<IOrder>
    {
        public async Task Consume(ConsumeContext<IOrder> context)
        {
            await Console.Out.WriteLineAsync($"Microservice_Storage收到订单：{Newtonsoft.Json.JsonConvert.SerializeObject(context.Message)}");
        }
    }
}
