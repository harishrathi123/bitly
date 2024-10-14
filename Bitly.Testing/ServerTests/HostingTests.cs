namespace Bitly.IntegrationTests;

public class HostingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    public HostingTests(WebApplicationFactory<Program> waf)
    {
        this._factory = waf;
    }

    [Fact]
    public async Task ShouldHostServer()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}