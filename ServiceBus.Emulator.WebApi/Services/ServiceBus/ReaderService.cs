using Azure.Messaging.ServiceBus;
using ConfigChecker.Shared.ServiceBus;

namespace ServiceBus.Emulator.WebApi.Services.ServiceBus;

public sealed class ReaderService(IClientManager clientManager) : IReaderService
{
    private ServiceBusClient client = clientManager.GetServiceBusClient();

    public async Task PeekTopMessagesInQueue(string queue, int numOfMessagesToPeak = 5)
    {
        ServiceBusReceiver receiver = client.CreateReceiver(queue);

        try
        {
            Console.WriteLine($"Attempting to peek top {numOfMessagesToPeak} massages from {queue}");

            // Peek operation with max count set to 5
            var peekedMessages = await receiver.PeekMessagesAsync(maxMessages: numOfMessagesToPeak);

            // Keep receiving while there are messages in the queue
            while (peekedMessages.Count > 0)
            {
                int counter = 0; // To get the sequence number of the last peeked message
                int countPeekedMessages = peekedMessages.Count;

                if (countPeekedMessages > 0)
                {
                    // For each peeked message, print the message body
                    foreach (ServiceBusReceivedMessage msg in peekedMessages)
                    {
                        Console.WriteLine(msg.Body);
                        counter++;
                    }
                    Console.WriteLine("Peek round complete");
                    Console.WriteLine("");
                }

                // Start receiving from the message after the last one
                var fromSeqNum = peekedMessages[counter - 1].SequenceNumber + 1;
                peekedMessages = await receiver.PeekMessagesAsync(maxMessages: numOfMessagesToPeak, fromSequenceNumber: fromSeqNum);

                Console.WriteLine("Peek done");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            await receiver.DisposeAsync();
            Console.WriteLine($"{nameof(ReaderService)} receiver disposed");
        }
    }
}
