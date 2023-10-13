using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;

namespace MicroService.Api.BusinessService.Services
{
    public class MessageConnection
    {
        public IConnection GetConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5432,
                Password = "guest",
                UserName = "guest",
                VirtualHost = "/"
            };

            return factory.CreateConnection();
        }
    }
}
