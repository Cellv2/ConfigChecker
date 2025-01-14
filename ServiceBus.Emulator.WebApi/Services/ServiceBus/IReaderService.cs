namespace ServiceBus.Emulator.WebApi.Services.ServiceBus;

public interface IReaderService
{
    Task PeekTopMessagesInQueue(string queue, int numOfMessagesToPeak = 5);
}