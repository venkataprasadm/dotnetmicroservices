using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName= _configuration["RabbitMQHost"],
                Port=  int.Parse(_configuration["RabbitMQPort"])
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type : ExchangeType.Fanout);
                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
                Console.WriteLine("--> connected to message bus");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Could not connect to Rabbit MQ " + ex.Message.ToString());
            }
        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connectionshutdown");
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
           var message = JsonSerializer.Serialize(platformPublishedDto);

           if(_connection.IsOpen)
           {
            Console.WriteLine("--> RabbitMQ Connection Open -- Writing message");
            SendMessage(message);
           }
           else
           {
            Console.WriteLine("--> Rabbit MQ Connection not opened, failed");
           }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger", routingKey: "",
                basicProperties: null,
                body: body);
            Console.WriteLine($"--> Sending Message {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("Message bus is closed");
            if(_channel.IsOpen){
                _channel.Close();
                _connection.Close();
            }
        }
    }
}