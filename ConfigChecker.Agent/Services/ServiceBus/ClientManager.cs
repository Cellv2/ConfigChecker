using Azure.Messaging.ServiceBus;

namespace ServiceBus.Emulator.WebApi.Services.ServiceBus;

public sealed class ClientManager : IClientManager
{
    private readonly ServiceBusClient client;

    private const string sbConnectionString = "Endpoint=sb://servicebus-emulator;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;";

    public ClientManager()
    {
        client = new ServiceBusClient(sbConnectionString);
    }

    public ServiceBusClient GetServiceBusClient() => client;
}
