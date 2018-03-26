using IntegrationEvents;
using Microservice_BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_BackgroundTask.Model
{
    public interface IEventRepository
    {
        Task<IEnumerable<OrderEventEntity>> GetEvent();

        Task<bool> UpdateOrderEnvent(IOrderEventEntity orderEvent);
    }
}
