using IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_Ship.Model
{
    public interface IShipRepository
    {
         Task<bool> CreateShip(IOrder order);
    }
}
