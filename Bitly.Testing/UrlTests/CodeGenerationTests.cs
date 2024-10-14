using FakeItEasy;
using Microsoft.Extensions.Options;
using Bitly.UrlServices;

namespace Bitly.Testing.UrlTests;

public class CodeGenerationTests
{
    private readonly CodeGeneration _codeGeneration;

    public CodeGenerationTests()
    {
        var config = new UrlShortnerConfig
        {
            SiteUrl = "https://example.com",
            Charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789",
            CodeLength = 8
        };

        var fakeOptions = A.Fake<IOptions<UrlShortnerConfig>>();
        A.CallTo(() => fakeOptions.Value).Returns(config);

        _codeGeneration = new CodeGeneration(fakeOptions);
    }

    [Fact]
    public void BaseUrl_ShouldReturnConfiguredSiteUrl()
    {
        // Act
        var result = _codeGeneration.BaseUrl();

        // Assert
        result.Should().Be("https://example.com");
    }

    [Fact]
    public void GenerateCode_ShouldReturnCodeOfConfiguredLength()
    {
        // Act
        var result = _codeGeneration.GenerateCode();

        // Assert
        result.Length.Should().Be(8);
    }
}
