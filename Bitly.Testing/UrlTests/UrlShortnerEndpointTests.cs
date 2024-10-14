using Bitly.UrlServices;

namespace Bitly.IntegrationTests;

public class UrlShortnerEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UrlShortnerEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_ShortensUrl_ReturnsShortUrl()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new UrlShortnerRequest { Url = "https://example.com" };

        // Act
        var response = await client.PostAsJsonAsync("/api/shorten", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var shortUrlResponse = await response.Content.ReadFromJsonAsync<ShortUrlResponse>();
        shortUrlResponse.Should().NotBeNull();
        shortUrlResponse!.OriginalUrl.Should().Be(request.Url);
        shortUrlResponse.ShortUrl.Should().NotBeNullOrEmpty();
        shortUrlResponse.Code.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Post_DuplicateUrl_ReturnsSameShortUrl()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new UrlShortnerRequest { Url = "https://example.com" };

        // Act
        var response1 = await client.PostAsJsonAsync("/api/shorten", request);
        var response2 = await client.PostAsJsonAsync("/api/shorten", request);

        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.OK);
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        var shortUrlResponse1 = await response1.Content.ReadFromJsonAsync<ShortUrlResponse>();
        var shortUrlResponse2 = await response2.Content.ReadFromJsonAsync<ShortUrlResponse>();

        shortUrlResponse1.Should().NotBeNull();
        shortUrlResponse2.Should().NotBeNull();

        shortUrlResponse1!.ShortUrl.Should().Be(shortUrlResponse2!.ShortUrl);
    }

    [Fact]
    public async Task Post_InvalidUrlFormat_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new UrlShortnerRequest { Url = "invalid-url" };

        // Act
        var response = await client.PostAsJsonAsync("/api/shorten", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        problemDetails.Should().NotBeNull();
        problemDetails!.Detail.Should().Contain("validation errors");
    }
}
