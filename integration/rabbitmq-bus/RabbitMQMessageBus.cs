using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace rabbitmq_bus
{
    public class RabbitMQMessageBus : IRabbitMQMessageBus
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private IConnection? _connection;

        public RabbitMQMessageBus()
        {
            _hostName = "localhost";
            _password = "guest";
            _userName = "guest";
        }

        public void SendMessage(object message, string topicQueueName)
        {
            if (ConnectionExists())
            {
                using var channel = _connection!.CreateModel();

                channel.QueueDeclare(topicQueueName, false, false, false, null);

                var jsonMessage = JsonConvert.SerializeObject(message);

                var body = Encoding.UTF8.GetBytes(jsonMessage);

                channel.BasicPublish(exchange: "", routingKey: topicQueueName, null, body);
            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _hostName,
                    UserName = _userName,
                    Password = _password
                };

                _connection = factory.CreateConnection();
            }
            catch (Exception) { }
        }

        private bool ConnectionExists()
        {
            if (_connection != null) return true;

            CreateConnection();

            return true;
        }
    }
}