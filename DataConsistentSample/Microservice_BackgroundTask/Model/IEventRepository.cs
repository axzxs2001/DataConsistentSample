using Microservice_BackgroundTask.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_Order.Model
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventEntity>> GetEvent();
    }
}
