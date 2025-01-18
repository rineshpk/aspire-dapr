using Aspire.Hosting.Dapr;
using k8s.Models;

var builder = DistributedApplication.CreateBuilder(args);

var launchProfileName = ShouldUseHttpForEndpoints() ? "http" : "https";

var stateStore = builder.AddDaprStateStore("statestore");
var pubSub = builder.AddDaprPubSub("pubsub");
var secretStore = builder.AddDaprComponent("secretstore", "secretstores.local.file", new DaprComponentOptions
{
    LocalPath = "..\\dapr\\components\\secretstore.yaml"
});


// Redis is used for pubsub and state store by default. So the redis needs to be running at the port 6379.
// Uncomment the below code to add redis as a container if it is not running
// builder.AddContainer("redis", "redis").WithEndpoint(port: 6379, targetPort: 6379);

var serviceA = builder.AddProject<Projects.AspireWithDapr_ServiceA>("service-a", launchProfileName)
    .WithDaprSidecar(options =>
        options.WithAnnotation(new EnvironmentCallbackAnnotation("APP_API_TOKEN", () => "secret-dapr-api-token"), ResourceAnnotationMutationBehavior.Append))
    .WithReference(stateStore)
    .WithReference(pubSub)
    .WithReference(secretStore)
    .WithEnvironment("APP_API_TOKEN", "secret-dapr-api-token");


var serviceB = builder.AddProject<Projects.AspireWithDapr_ServiceB>("service-b", launchProfileName)
    .WithDaprSidecar(options =>
        options.WithAnnotation(new EnvironmentCallbackAnnotation("APP_API_TOKEN", () => "secret-dapr-api-token"), ResourceAnnotationMutationBehavior.Append))
    .WithReference(stateStore)
    .WithReference(pubSub)
    .WithReference(secretStore)
    .WithEnvironment("APP_API_TOKEN", "secret-dapr-api-token");


builder.AddProject<Projects.AspireWithDapr_WebUI>("webfrontend")
       .WithDaprSidecar(options =>
            options.WithAnnotation(new EnvironmentCallbackAnnotation("APP_API_TOKEN", () => "secret-dapr-api-token"), ResourceAnnotationMutationBehavior.Append))
       .WaitFor(serviceA)
       .WaitFor(serviceB)
       .WithEnvironment("APP_API_TOKEN", "secret-dapr-api-token"); 


builder.Build().Run();

static bool ShouldUseHttpForEndpoints()
{
    const string EnvVarName = "USE_HTTP_ENDPOINTS";
    var envValue = Environment.GetEnvironmentVariable(EnvVarName);

    // Attempt to parse the environment variable value; return true if it's exactly "1".
    return int.TryParse(envValue, out int result) && result == 1;
}

