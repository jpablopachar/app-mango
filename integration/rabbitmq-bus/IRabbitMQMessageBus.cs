namespace rabbitmq_bus
{
    public interface IRabbitMQMessageBus
    {
        void SendMessage(object message, string topicQueueName);
    }
}