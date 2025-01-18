using Dapr.Client;

namespace AspireWithDapr.WebUI;

public class ApiClient(DaprClient daprClient)
{
    public async Task<string> GetDataAsync()
    {
        return await daprClient.InvokeMethodAsync<string>(HttpMethod.Get, "service-b", "get-data");
    }
}

