using Microsoft.AspNetCore.Mvc.Testing;

namespace market.IntegrationTests.MVC;

public sealed class PageSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PageSmokeTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/shop")]
    [InlineData("/components")]
    [InlineData("/store")]
    [InlineData("/login")]
    [InlineData("/register")]
    public async Task Page_IsServed(string url)
    {
        var response = await _client.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}
