using Dapr.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.Services.AddSwaggerGen();

builder.Services.AddDaprClient();

// Adds Dapr authentication. By default the token will be read from APP_API_TOKEN environment variable.
builder.Services.AddAuthentication().AddDapr();

builder.Services.AddAuthorization(options =>
{
    // Adds Dapr authorization. Authentication scheme is "Dapr"
    options.AddDapr();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapDefaultEndpoints();

app.MapGet("/get-data", async (DaprClient daprClient) =>
{
    // Call ServiceB using Dapr Service Invocation
    var response = await daprClient.InvokeMethodAsync<string>(HttpMethod.Get,"service-b", "get-data");
    return Results.Ok(new { Message = "Data from ServiceB", Response = response });
});

app.MapPost("/publish", async (DaprClient daprClient, string message) =>
{
    // Publish a message to the Pub/Sub topic
    await daprClient.PublishEventAsync("pubsub", "sample-topic", message);
    return Results.Ok("Message published!");
});

app.Run();
