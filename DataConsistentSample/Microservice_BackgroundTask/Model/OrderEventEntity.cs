using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice_BackgroundTask.Model
{
    public class OrderEventEntity
    {
        public int ID { get; set; }
        public string EventType { get; set; }
        public string OrderID { get; set; }
        public DateTime CreateTime { get; set; }
        public int ShipStatus { get; set; }
        public int StorageStatus { get; set; }
        public string EntityJson { get; set; }
    }
}
