using MassTransit;
using System;
using System.Collections.Generic;

namespace IntegrationEvents
{

    public interface IOrder
    {
        string ID { get; set; }

        DateTime OrderTime { get; set; }

        List<IOrderItem> OrderItmes{get;set;}

        string OrderUserID { get; set; }
        
    }

}
