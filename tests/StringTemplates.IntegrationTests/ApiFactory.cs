using Microsoft.AspNetCore.Mvc.Testing;

namespace StringTemplates.IntegrationTests;

public class ApiFactory : WebApplicationFactory<Program>
{
    public HttpClient CreateHttpsClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }
}
