using Microsoft.Extensions.Azure;
using ServiceBus.Emulator.WebApi.Services.ServiceBus;

// check this for some examples of stuff:
// https://github.com/Azure/azure-service-bus-emulator-installer/blob/main/Sample-Code-Snippets/NET/ServiceBus.Emulator.Console.Sample/ServiceBus.Emulator.Console.Sample/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();
builder.Services.AddSingleton<IClientManager, ClientManager>();
builder.Services.AddSingleton<IEnqueueService, EnqueueService>();
builder.Services.AddHealthChecks().AddAzureServiceBusQueue("Endpoint=sb://servicebus-emulator;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=SAS_KEY_VALUE;UseDevelopmentEmulator=true;", "queue.1");
var app = builder.Build();

var enqueueService = app.Services.GetRequiredService<IEnqueueService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapHealthChecks("/healthz");

app.MapPost("/testSendToQueue", async (TestQueueSubmission queueSubmission) =>
{
    try
    {
        await enqueueService.CreateBatchInQueue(queueSubmission.QueueName);
        return Results.Ok(queueSubmission);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }

});

app.MapPost("/sendClientCodeToQueue", async (ClientCodeQueueSubmission queueSubmission) => {
    try
    {
        await enqueueService.SendClientCodeToSpecifiedQueue(queueSubmission.QueueName, queueSubmission.ClientCode);
        return Results.Ok(queueSubmission);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex);
    }
});

app.Run();

record TestQueueSubmission(string QueueName);

record ClientCodeQueueSubmission(string QueueName, string ClientCode);



record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
