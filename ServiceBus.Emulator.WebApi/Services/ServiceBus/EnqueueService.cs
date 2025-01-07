using Azure.Messaging.ServiceBus;

namespace ServiceBus.Emulator.WebApi.Services.ServiceBus;

public sealed class EnqueueService(IClientManager clientManager) : IEnqueueService
{
    private ServiceBusClient client = clientManager.GetServiceBusClient();
    private const int numOfMessages = 3;

    public async Task CreateBatchInQueue(string queueName)
    {
        ServiceBusSender sender = client.CreateSender(queueName);

        using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

        for (int i = 1; i <= numOfMessages; i++)
        {
            // try adding a message to the batch
            if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
            {
                // if it is too large for the batch
                throw new Exception($"The message {i} is too large to fit in the batch.");
            }
        }

        try
        {
            // Use the producer client to send the batch of messages to the Service Bus queue
            await sender.SendMessagesAsync(messageBatch);
            Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            // Calling DisposeAsync on client types is required to ensure that network
            // resources and other unmanaged objects are properly cleaned up.
            await sender.DisposeAsync();
            //await client.DisposeAsync();
        }

        //const int numOfMessagesPerBatch = 10;
        //const int numOfBatches = 10;

        ////var client = new ServiceBusClient(_connectionString);
        ////var sender = client.CreateSender(queueName);
        //var sender = client.CreateSender("queue.1");

        //await sender.SendMessageAsync(new ServiceBusMessage("123123"));

        //for (int i = 1; i <= numOfBatches; i++)
        //{
        //    using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

        //    for (int j = 1; j <= numOfMessagesPerBatch; j++)
        //    {
        //        messageBatch.TryAddMessage(new ServiceBusMessage($"Batch:{i}:Message:{j}"));
        //    }
        //    await sender.SendMessagesAsync(messageBatch);
        //}

        //await sender.DisposeAsync();
    }

    public async Task SendClientCodeToSpecifiedQueue(string queueName, string clientCode)
    {
        ServiceBusSender sender = client.CreateSender(queueName);

        try
        {
            await sender.SendMessageAsync(new ServiceBusMessage(clientCode));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            await sender.DisposeAsync();
        }
    }
}
