using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationEvents;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Microservice_Order.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        IBusControl _bus;
        public OrderController(IBusControl bus)
        {
            _bus = bus;
        }
       
        [HttpPost]
        public void Post([FromBody]Order order)
        {
            _bus.Publish(order);
        }

      
    }
}
