using ConfigChecker.Agent.Services;
using ConfigChecker.Agent.Services.ConfigProcessor;
using ConfigChecker.Agent.Services.ConfigProcessor.ConfigToObjectMappers;
using ConfigChecker.Agent.Services.ServiceBus;
using ConfigChecker.Shared.ServiceBus;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IAgentFileService, AgentFileService>();
builder.Services.AddSingleton<IClientManager, ClientManager>();
builder.Services.AddSingleton<IConfigMappingProcessor, ConfigToObjectProcessor>();
builder.Services.AddTransient<IConfigToObjectMapperBase, JsonConfigMapper>();
builder.Services.AddTransient<IConfigToObjectMapperBase, XmlConfigMapper>();
builder.Services.AddSingleton<IConsumerService, ConsumerService>();
builder.Services.AddSingleton<IResponseService, ResponseService>();
var app = builder.Build();

var agentFileService = app.Services.GetRequiredService<IAgentFileService>();
var configMappingProcessor = app.Services.GetRequiredService<IConfigMappingProcessor>();
var consumer = app.Services.GetRequiredService<IConsumerService>();
var responseService = app.Services.GetRequiredService<IResponseService>();

const string queueName = "queue.1";

var hostApplicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
hostApplicationLifetime.ApplicationStarted.Register(async () => await consumer.ConsumeQueueMessagesAsync(queueName));
hostApplicationLifetime.ApplicationStopping.Register(async () => await consumer.Shutdown());

app.MapGet("/api/{clientCode}", async (string clientCode) => {
    var configObjects = configMappingProcessor.ProcessConfigsPathsToObjects(agentFileService.GetConfigFileValue(clientCode));
    var json = JsonSerializer.Serialize(configObjects);
    await responseService.Send(json);

    return new { Message = configObjects };
});

app.MapGet("/", () => "Hello World!");

app.Run();
