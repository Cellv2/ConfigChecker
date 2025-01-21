using Azure.Messaging.ServiceBus;
using ConfigChecker.Analysis.Services.HttpClients;
using ConfigChecker.Analysis.Services.SecureValueAccess;
using ServiceBus.Emulator.WebApi.Services.ServiceBus;
using System.Text.Json;

namespace ConfigChecker.Analysis.Services.ServiceBus;

public sealed class ConsumerService(IClientManager clientManager, ISecureValueAccessHttpClient secureValueAccessHttpClient, ISecureValueAccessService secureValueAccessService) : IConsumerService
{
    private ServiceBusClient client = clientManager.GetServiceBusClient();
    private bool shouldProcessQueueMessages = false;

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    public async Task ConsumeQueueMessagesAsync(string queueName)
    {
        ServiceBusProcessor processor = client.CreateProcessor(queueName);

        try
        {
            // add handler to process messages
            processor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            processor.ProcessErrorAsync += ErrorHandler;

            shouldProcessQueueMessages = true;

            // start processing 
            Console.WriteLine($"Starting queue processing for {queueName}");
            while (shouldProcessQueueMessages)
            {
                if (!processor.IsProcessing)
                {
                    await processor.StartProcessingAsync(tokenSource.Token);
                    Console.WriteLine($"Started queue processing for {queueName} successfully");
                }
            }

            // stop processing 
            Console.WriteLine("\nStopping the receiver...");
            await processor.StopProcessingAsync();
            await processor.CloseAsync();
            Console.WriteLine("Stopped receiving messages");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            tokenSource.Cancel();
            shouldProcessQueueMessages = false;
            await processor.DisposeAsync();
        }
    }

    public async Task Shutdown()
    {
        Console.WriteLine("Shutting down consumer");
        shouldProcessQueueMessages = false;
        tokenSource.Cancel();

        await Task.Yield();
    }

    // handle received messages
    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string receivedConfigValues = args.Message.Body.ToString();
        Console.WriteLine($"Received: {receivedConfigValues}");

        // TODO: flatten the values on the agent side?
        // TODO: maybe add a fileName k:v pair agent side?
        var deserializedConfigValues = JsonSerializer.Deserialize<List<Dictionary<string, dynamic>>>(receivedConfigValues).ToArray();

        if (deserializedConfigValues == null)
        {
            Console.WriteLine($"Panic! Couldn't deserialize config values - config was {receivedConfigValues}");

            // remove the queue item, else we'll just get a loop of this failing anyway
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        Console.WriteLine("TODO: process this now please!");
        //await secureValueAccessHttpClient.GetValueForKeyFromRedis("key1");

        List<Task<bool>> tasks = new();
        foreach (var configValueDictionary in deserializedConfigValues)
        {
            foreach (KeyValuePair<string, dynamic> pair in configValueDictionary)
            {
                tasks.Add(secureValueAccessService.DoesConfigValueMatch(pair.Key, pair.Value));
            }
        }
        await Task.WhenAll(tasks);

        // complete the message. message is deleted from the queue. 
        await args.CompleteMessageAsync(args.Message);
    }

    // handle any errors when receiving messages
    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}
