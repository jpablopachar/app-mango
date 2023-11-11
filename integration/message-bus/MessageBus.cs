using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace message_bus
{
    public class MessageBus : IMessageBus
    {
        private readonly string connectionString = "Endpoint=sb://mango-app.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=Fd877r/Lu0ZxYvYn3WUJTa7WXK/IzaKOE+ASbP0jDFo=";

        /// <summary>Publishes a message to a Service Bus topic or queue using a provided connection string and topic/queue name.</summary>
        /// <param name="message">Represents the message you want to publish to the Service Bus topic or queue.</param>
        /// <param name="topicQueueName">Is the name of the topic or queue to which you want to publish the message.</param>
        public async Task PublishMessageAsync(object message, string topicQueueName)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topicQueueName);

            var jsonMessage = JsonConvert.SerializeObject(message);

            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding
                .UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}