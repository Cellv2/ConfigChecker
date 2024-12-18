using ConfigChecker.Agent.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var fileService = new AgentFileService();

app.MapGet("/{clientCode}", (string clientCode) => new { Message = fileService.GetConfigFileValue(clientCode) });

app.MapGet("/", () => "Hello World!");

app.Run();
