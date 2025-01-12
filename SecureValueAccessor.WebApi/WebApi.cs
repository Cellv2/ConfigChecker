using NRedisStack;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();




app.MapPost("/getRedisValueByKey", async (X theThings) =>
{
    //ConfigurationOptions configurationOptions = new()
    //{
    //    EndPoints = { { "data.redis", 7379 } }
    //};
    //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(configurationOptions);
    //ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("data.redis:7379");
    ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("data.redis:6379");
    IDatabase db = redis.GetDatabase();

    var redisValue = await db.StringGetAsync(theThings.Key);
    if (redisValue.HasValue == false)
    {
        return Results.NotFound($"No value found for key '{theThings.Key}'");
    }

    return Results.Ok(redisValue);
});


app.Run();

public record X(string Key);