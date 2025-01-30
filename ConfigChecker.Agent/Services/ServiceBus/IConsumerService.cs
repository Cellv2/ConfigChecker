
namespace ConfigChecker.Agent.Services.ServiceBus;

public interface IConsumerService
{
    Task ConsumeQueueMessagesAsync(string queueName);
    Task Shutdown();
}