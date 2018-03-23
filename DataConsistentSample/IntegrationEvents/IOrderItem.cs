using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationEvents
{
    public interface IOrderItem
    {
        string ID { get; set; }

        string ProductID { get; set; }
        decimal Price { get; set; }
        double Quantity { get; set; }
    }
}
