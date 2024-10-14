using Bitly.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

namespace Bitly.IntegrationTests;

public class UrlRedirectEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public UrlRedirectEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_ValidCode_RedirectsToOriginalUrl()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var shortnedUrl = new ShortnedUrl
        {
            OriginalUrl = "https://example.com",
            ShortUrl = "https://short.ly/abc123",
            Code = "abc123",
            CreatedOn = DateTime.UtcNow
        };
        dbContext.ShortnedUrls.Add(shortnedUrl);
        await dbContext.SaveChangesAsync();

        // Act
        var response = await client.GetAsync("/api/abc123");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location.Should().Be(shortnedUrl.OriginalUrl);
    }

    [Fact]
    public async Task Get_InvalidCode_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/invalidcode");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_CachedCode_RedirectsToOriginalUrl()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var cache = _factory.Services.GetRequiredService<IMemoryCache>();
        var shortnedUrl = new ShortnedUrl
        {
            OriginalUrl = "https://example.com",
            ShortUrl = "https://short.ly/abc123",
            Code = "abc123",
            CreatedOn = DateTime.UtcNow
        };
        dbContext.ShortnedUrls.Add(shortnedUrl);
        await dbContext.SaveChangesAsync();
        cache.Set("abc123", shortnedUrl.OriginalUrl, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        });

        // Act
        var response = await client.GetAsync("/api/abc123");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location.Should().Be(shortnedUrl.OriginalUrl);
    }
}
