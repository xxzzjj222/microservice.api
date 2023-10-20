using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace MicroService.Api.ProductService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "ˮ��", "ɳ��", "ƻ��", "���� ", "����", "����", "â��", "������", "ӣ��", "����"
        };

        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void CreateProduct()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/"
            };

            var connection=factory.CreateConnection();
            var channel=connection.CreateChannel();
            channel.QueueDeclare("product-create", false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                Console.WriteLine($"Model:{sender}");
                var body = args.Body;
                var message = Encoding.UTF8.GetString(body.ToArray());
                Console.WriteLine($"������Ʒ:{0}", message);
                channel.BasicAck(args.DeliveryTag, true);
            };

            channel.BasicQos(0, 1, false);
            channel.BasicConsume("product-create", false, consumer);
        }
    }
}