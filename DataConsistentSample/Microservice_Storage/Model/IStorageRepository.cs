using IntegrationEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_Storage.Model
{
    public interface IStorageRepository
    {
        Task<bool> CreateStorage(IOrder order);
    }
}
