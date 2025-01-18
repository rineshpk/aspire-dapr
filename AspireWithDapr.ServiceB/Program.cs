using Dapr.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Decodes cloud events sent to the `MapPost` below.
app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/get-data", async (DaprClient daprClient) =>
{
    // Retrieve cached data from the state store
    var state = await daprClient.GetStateAsync<string>("statestore", "cached-key");
    if (string.IsNullOrEmpty(state))
    {
        state = "Data Form Service B";
        await daprClient.SaveStateAsync("statestore", "cached-key", state);
    }

    return Results.Ok(state);
});

app.MapPost("/subscribe", async ([FromBody] string message, ILogger<Program> logger) =>
{
    // Handle Pub/Sub messages
    logger.LogInformation("Received message: {Message}", message);
    return Results.Ok($"Message received: {message}");

})
.WithTopic("pubsub", "sample-topic")
.RequireAuthorization(new AuthorizeAttribute { AuthenticationSchemes = "Dapr" });

app.MapGet("/get-secret", async (DaprClient daprClient) =>
{
    // Retrieve a secret from the secret store
    var secret = await daprClient.GetSecretAsync("secretstore", "my-secret");
    return Results.Ok(secret);
});

app.Run();
