
namespace ConfigChecker.Agent.Services.ServiceBus
{
    public interface IConsumerService1
    {
        Task ConsumeQueueMessagesAsync(string queueName);
        Task Shutdown();
    }
}