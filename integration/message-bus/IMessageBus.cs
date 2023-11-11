namespace message_bus
{
    public interface IMessageBus
    {
        Task PublishMessageAsync(object message, string topicQueueName);
    }
}