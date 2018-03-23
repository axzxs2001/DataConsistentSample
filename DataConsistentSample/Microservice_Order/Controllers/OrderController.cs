using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IntegrationEvents;
using MassTransit;
using Microservice_Order.Model;
using Microsoft.AspNetCore.Mvc;

namespace Microservice_Order.Controllers
{
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        IBusControl _bus;
        IOrderRepository _orderRepository;
        public OrderController(IBusControl bus, IOrderRepository orderRepository)
        {
            _bus = bus;
            _orderRepository = orderRepository;
        }

        [HttpPost]
        public void Post([FromBody]Order order)
        {
            var result = _orderRepository.CreateOrder(order).GetAwaiter().GetResult();
            if (result)
            {
                _bus.Publish(order);
            }
        }


    }
}
