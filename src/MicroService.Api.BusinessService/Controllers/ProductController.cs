using MicroService.Api.BusinessService.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json.Serialization;

namespace MicroService.Api.BusinessService.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<ProductController> _logger;

    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<Product> Get()
    {
        return Enumerable.Range(1, 5).Select(index => new Product
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpPost]
    public void CreateProduct(ProductCreateDto productCreateDto)
    {
        //1. 创建连接
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/"
        };
        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        //2. 定义队列
        channel.QueueDeclare("product_create", false, false, false, null);
        string productJson = JsonConvert.SerializeObject(productCreateDto);
        var body = Encoding.UTF8.GetBytes(productJson);

        //3. 发送消息
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish("", "product_create", properties, body);

        //4. 记录日志
        _logger.LogInformation("成功创建商品");
    }

    [HttpPost("fanout")]
    public void CreateProductFanout(ProductCreateDto productCreateDto)
    {
        //1. 创建连接
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/"
        };
        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        //2. 定义队列
        channel.ExchangeDeclare("product_fanout", "fanout");
        string productJson= JsonConvert.SerializeObject(productCreateDto);
        var body= Encoding.UTF8.GetBytes(productJson);

        //3. 发送消息
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish("product_fanout", "product_fanout", properties, body);

        //4. 记录日志
        _logger.LogInformation("成功创建商品");
    }

    [HttpPost("direct")]
    public void CreateProductDirect(ProductCreateDto productCreateDto)
    {
        //1. 创建连接
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/"
        };
        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        //2. 定义队列
        channel.ExchangeDeclare("product_direct", "direct");
        string productJson = JsonConvert.SerializeObject(productCreateDto);
        var body = Encoding.UTF8.GetBytes(productJson);

        //3. 发送消息
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish("product_direct", "product_direct", properties, body);

        //4. 记录日志
        _logger.LogInformation("成功创建商品");
    }

    [HttpPost("topic")]
    public void CreateProductTopic(ProductCreateDto productCreateDto)
    {
        //1. 创建连接
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/"
        };
        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        //2. 定义队列
        channel.ExchangeDeclare("product_topic", "topic");
        string productJson = JsonConvert.SerializeObject(productCreateDto);
        var body = Encoding.UTF8.GetBytes(productJson);

        //3. 发送消息
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        channel.BasicPublish("product_topic", "product_topic", properties, body);

        //4. 记录日志
        _logger.LogInformation("成功创建商品");
    }

    [HttpPost("event")]
    public void CreateProductEvent(ProductCreateDto productCreateDto)
    {
        //1. 创建连接
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/"
        };
        using var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        //2. 定义队列
        var replyQueueName = channel.QueueDeclare().QueueName;
        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        var correctionId = Guid.NewGuid().ToString();
        properties.CorrelationId= correctionId;
        properties.ReplyTo = replyQueueName;

        string productJson = JsonConvert.SerializeObject(productCreateDto);
        var body = Encoding.UTF8.GetBytes(productJson);

        //3. 发送消息
        channel.BasicPublish("", "product_event", properties, body);

        //4. 消息回调
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            Console.WriteLine($"model:{model}");
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            if(ea.BasicProperties.CorrelationId== correctionId)
            {
                Console.WriteLine(" [x] 回调成功 {0}", message);
            }
        };

        //5. 消费消息
        channel.BasicConsume(replyQueueName, true, consumer);

        //6. 记录日志
        _logger.LogInformation("成功创建商品");
    }
}
