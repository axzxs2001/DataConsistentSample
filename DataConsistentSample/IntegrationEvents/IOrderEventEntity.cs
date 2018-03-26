using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationEvents
{
    public interface IOrderEventEntity
    {
         int ID { get; set; }
         string EventType { get; set; }
         string OrderID { get; set; }
         DateTime CreateTime { get; set; }
         int ShipStatus { get; set; }
         int StorageStatus { get; set; }
         string EntityJson { get; set; }
    }
}
