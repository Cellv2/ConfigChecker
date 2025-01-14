using Azure.Messaging.ServiceBus;
using ServiceBus.Emulator.WebApi.Services.ServiceBus;

namespace ConfigChecker.Agent.Services.ServiceBus;

public sealed class ResponseService(IClientManager clientManager) : IResponseService
{
    private ServiceBusClient client = clientManager.GetServiceBusClient();

    private const string queueName = "queue.2";

    public async Task Send(string message)
    {
        ServiceBusSender processor = client.CreateSender(queueName);

        try
        {
            await processor.SendMessageAsync(new(message));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            await processor.DisposeAsync();
        }
    }
}
