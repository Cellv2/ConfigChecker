using ConfigChecker.Agent.Services;
using ConfigChecker.Agent.Services.ConfigProcessor;
using ConfigChecker.Agent.Services.ConfigProcessor.ConfigToObjectMappers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IAgentFileService, AgentFileService>();
builder.Services.AddSingleton<IConfigMappingProcessor, ConfigToObjectProcessor>();
builder.Services.AddTransient<IConfigToObjectMapperBase, JsonConfigMapper>();
builder.Services.AddTransient<IConfigToObjectMapperBase, XmlConfigMapper>();
var app = builder.Build();

var agentFileService = app.Services.GetRequiredService<IAgentFileService>();
var configMappingProcessor = app.Services.GetRequiredService<IConfigMappingProcessor>();

app.MapGet("/api/{clientCode}", (string clientCode) => new { Message = configMappingProcessor.ProcessConfigsPathsToObjects(agentFileService.GetConfigFileValue(clientCode)) });

app.MapGet("/", () => "Hello World!");

app.Run();
