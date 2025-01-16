using ConfigChecker.Analysis.Services.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceBus.Emulator.WebApi.Services.ServiceBus;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddSingleton<IClientManager, ClientManager>();
        services.AddSingleton<IConsumerService, ConsumerService>();
    })
    .Build();

var consumer = host.Services.GetRequiredService<IConsumerService>();

const string responseQueueName = "queue.2";

var hostApplicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
hostApplicationLifetime.ApplicationStarted.Register(async () => await consumer.ConsumeQueueMessagesAsync(responseQueueName));
hostApplicationLifetime.ApplicationStopping.Register(async () => await consumer.Shutdown());

host.Run();