using Azure.Messaging.ServiceBus;

namespace ServiceBus.Emulator.WebApi.Services.ServiceBus
{
    public interface IClientManager
    {
        ServiceBusClient GetServiceBusClient();
    }
}