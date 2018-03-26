using IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_Order.Model
{
    public interface IOrderEventRepository
    {
         Task<bool> UpdateOrderEnvent(IOrderEventEntity orderEvent);
    }
}
