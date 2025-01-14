namespace ConfigChecker.Agent.Services.ServiceBus;

public interface IResponseService
{
    Task Send(string message);
}