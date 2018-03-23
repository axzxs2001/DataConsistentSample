using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MassTransit;

namespace IntegrationEvents
{


    public class Order:IOrder
    {
      public  string ID { get; set; }

        public DateTime OrderTime { get; set; }

        public List<IOrderItem> OrderItmes { get; set; }

        public string OrderUserID { get; set; }

      
    }
}
