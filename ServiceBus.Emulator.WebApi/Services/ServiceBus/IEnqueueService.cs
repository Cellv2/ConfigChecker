
namespace ServiceBus.Emulator.WebApi.Services.ServiceBus;

public interface IEnqueueService
{
    Task CreateBatchInQueue(string queueName);
    Task SendClientCodeToSpecifiedQueue(string queueName, string clientCode);
}