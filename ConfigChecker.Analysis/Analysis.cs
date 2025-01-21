using ConfigChecker.Analysis.Services.HttpClients;
using ConfigChecker.Analysis.Services.SecureValueAccess;
using ConfigChecker.Analysis.Services.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceBus.Emulator.WebApi.Services.ServiceBus;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IClientManager, ClientManager>();
        services.AddSingleton<IConsumerService, ConsumerService>();
        services.AddSingleton<ISecureValueAccessService, SecureValueAccessService>();

        // https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#avoid-typed-clients-in-singleton-services
        // https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory#using-ihttpclientfactory-together-with-socketshttphandler
        services.AddHttpClient<ISecureValueAccessHttpClient, SecureValueAccessHttpClient>()
                .UseSocketsHttpHandler((handler, _) => {
                    handler.SslOptions = new() { RemoteCertificateValidationCallback = delegate { return true; } }; // this is dev, we want to ignore SSL certs
                    handler.PooledConnectionLifetime = TimeSpan.FromMinutes(2); // Recreate connection every 2 minutes
                }) 
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan); // Disable rotation, as it is handled by PooledConnectionLifetime
    })
    .Build();

var consumer = host.Services.GetRequiredService<IConsumerService>();

const string responseQueueName = "queue.2";

var hostApplicationLifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
hostApplicationLifetime.ApplicationStarted.Register(async () => await consumer.ConsumeQueueMessagesAsync(responseQueueName));
hostApplicationLifetime.ApplicationStopping.Register(async () => await consumer.Shutdown());

host.Run();