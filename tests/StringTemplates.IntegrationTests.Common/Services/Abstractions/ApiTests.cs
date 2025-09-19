using System.Net.Http.Json;

// ReSharper disable once CheckNamespace
namespace StringTemplates.IntegrationTests.Common;

public abstract class ApiTests(ApiFactory factory)
{
    protected readonly HttpClient Client = factory.CreateHttpsClient();

    protected async Task<string> PostForStringAsync(string path, object payload)
    {
        var resp = await Client.PostAsJsonAsync(path, payload);
        resp.EnsureSuccessStatusCode();

        var jsonString = await resp.Content.ReadFromJsonAsync<string>();
        return jsonString ?? await resp.Content.ReadAsStringAsync();
    }
}
