using Microsoft.AspNetCore.Mvc.Testing;

namespace PoshakGhor.IntegrationTests.MVC;

public sealed class HomePageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HomePageTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Fact]
    public async Task HomePage_IsServedByApplicationHost()
    {
        var response = await _client.GetAsync("/");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("PoshakGhor", await response.Content.ReadAsStringAsync());
    }
}
