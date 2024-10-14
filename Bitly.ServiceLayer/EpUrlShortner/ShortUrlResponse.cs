using Bitly.Database;

namespace Bitly.UrlServices;

public class ShortUrlResponse
{
    public required string OriginalUrl { get; set; }
    public required string ShortUrl { get; set; }
    public required string Code { get; set; }

    public static ShortUrlResponse Create(UrlShortnerRequest request, string siteUrl, string code)
    {
        return new ShortUrlResponse
        {
            OriginalUrl = request.Url.ToLowerInvariant(),
            ShortUrl = $"{siteUrl.ToLowerInvariant()}{code}",
            Code = code
        };
    }

    public ShortnedUrl ToEntity()
    {
        return new ShortnedUrl
        {
            Id = Guid.NewGuid(),
            OriginalUrl = OriginalUrl.ToLowerInvariant(),
            ShortUrl = ShortUrl.ToLowerInvariant(),
            Code = Code,
            CreatedOn = DateTime.UtcNow
        };
    }

    public static ShortUrlResponse FromEntity(ShortnedUrl entity)
    {
        return new ShortUrlResponse
        {
            OriginalUrl = entity.OriginalUrl,
            ShortUrl = entity.ShortUrl,
            Code = entity.Code
        };
    }
}
