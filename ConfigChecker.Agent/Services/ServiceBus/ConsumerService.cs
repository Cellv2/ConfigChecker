using Azure.Messaging.ServiceBus;
using ServiceBus.Emulator.WebApi.Services.ServiceBus;

namespace ConfigChecker.Agent.Services.ServiceBus;

public sealed class ConsumerService(IClientManager clientManager) : IConsumerService
{
    private ServiceBusClient client = clientManager.GetServiceBusClient();
    private bool shouldProcessQueueMessages = false;

    CancellationTokenSource tokenSource = new CancellationTokenSource();

    public async Task ConsumeQueueMessagesAsync()
    {
        string queueName = "queue.1";
        ServiceBusProcessor processor = client.CreateProcessor(queueName);

        try
        {
            // add handler to process messages
            processor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            processor.ProcessErrorAsync += ErrorHandler;

            shouldProcessQueueMessages = true;

            // start processing 
            Console.WriteLine($"Queue processing for {queueName} has started");
            while (shouldProcessQueueMessages)
            {
                if (!processor.IsProcessing)
                {
                    await processor.StartProcessingAsync(tokenSource.Token);
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
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body}");

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
