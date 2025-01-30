using Azure.Messaging.ServiceBus;

namespace ConfigChecker.Shared.ServiceBus;

public interface IClientManager
{
    ServiceBusClient GetServiceBusClient();
}