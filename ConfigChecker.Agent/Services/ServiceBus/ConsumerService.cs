using Azure.Messaging.ServiceBus;
using ServiceBus.Emulator.WebApi.Services.ServiceBus;

namespace ConfigChecker.Agent.Services.ServiceBus;

public sealed class ConsumerService(IClientManager clientManager) : IConsumerService
{
    private ServiceBusClient client = clientManager.GetServiceBusClient();

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

            // start processing 
            await processor.StartProcessingAsync();
            Console.WriteLine($"Queue processing for {queueName} has started");

            //Console.WriteLine("Wait for a minute and then press any key to end the processing");
            //Console.ReadKey();
            // !!! TRADE OFFER !!!
            // I receive code that does what I want
            // You receive stupid code
            Console.Read();

            //// stop processing 
            //Console.WriteLine("\nStopping the receiver...");
            //await processor.StopProcessingAsync();
            //Console.WriteLine("Stopped receiving messages");
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
