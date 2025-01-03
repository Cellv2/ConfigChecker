
namespace ConfigChecker.Agent.Services.ServiceBus
{
    public interface IConsumerService
    {
        Task ConsumeQueueMessagesAsync();
        Task Shutdown();
    }
}