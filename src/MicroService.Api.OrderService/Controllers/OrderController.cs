using MicroService.Api.OrderService.Models.Dtos;
using MicroService.Api.OrderService.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MicroService.Api.OrderService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<Order> Get()
        {
            Order order = new Order();
            OrderStockDto orderStockDto = new OrderStockDto()
            {
                ProductId = 1,
                Stock = 1
            };

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateChannel();

            var queueName = channel.QueueDeclare().QueueName;
            channel.ExchangeDeclare("order_topic", "topic");
            channel.QueueBind(queueName, "order_topic", "sms.*");

            var consumer=new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                Console.WriteLine($"Model：{sender}");
                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine($"创建商品{0}", message);
                channel.BasicAck(args.DeliveryTag, true);
            };

            channel.BasicQos(0, 1, false);
            channel.BasicConsume(queueName, false, consumer);

            _logger.LogInformation("成功创建订单");

            return order;
        }
    }
}