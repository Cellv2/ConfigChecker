using ConfigChecker.Agent.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IAgentFileService, AgentFileService>();
var app = builder.Build();

var agentFileService = app.Services.GetRequiredService<IAgentFileService>();

app.MapGet("/{clientCode}", (string clientCode) => new { Message = agentFileService.GetConfigFileValue(clientCode) });

app.MapGet("/", () => "Hello World!");

app.Run();
