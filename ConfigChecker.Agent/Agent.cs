using ConfigChecker.Agent.Services;
using ConfigChecker.Agent.Services.ConfigProcessor;
using ConfigChecker.Agent.Services.ConfigProcessor.ConfigToObjectMappers;
using ConfigChecker.Agent.Services.ServiceBus;
using ServiceBus.Emulator.WebApi.Services.ServiceBus;

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

const string queueName = "queue.1";

var hostApplicationLifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
hostApplicationLifetime.ApplicationStarted.Register(async () => await consumer.ConsumeQueueMessagesAsync(queueName));
hostApplicationLifetime.ApplicationStopping.Register(async () => await consumer.Shutdown());

app.MapGet("/api/{clientCode}", (string clientCode) => new { Message = configMappingProcessor.ProcessConfigsPathsToObjects(agentFileService.GetConfigFileValue(clientCode)) });

app.MapGet("/", () => "Hello World!");

app.Run();
