using IntegrationEvents;
using MassTransit;
using Microservice_Storage.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_Storage.EventHandlers
{
    public class StoreageOrderEventHandler : IConsumer<IOrder>
    {
        IStorageRepository _storageRepository;
        public StoreageOrderEventHandler(IStorageRepository storageRepository)
        {
            _storageRepository = storageRepository;
        }
        public async Task Consume(ConsumeContext<IOrder> context)
        {
            await _storageRepository.CreateStorage(context.Message);
        }
    }
}
