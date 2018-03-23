using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationEvents
{
    public class OrderItem : IOrderItem
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public decimal Price { get; set; }
        public double Quantity { get; set; }
    }
}
