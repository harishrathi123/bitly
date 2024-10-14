using Bitly.Database;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Bitly.UrlServices;

public class UrlRedirectHandler: IRequestHandler<UrlRedirectRequest, Results<RedirectHttpResult, BadRequest, NotFound>>
{
    private readonly AppDbContext dbContext;
    private readonly IMemoryCache cache;
    private readonly ILogger<UrlRedirectHandler> logger;

    public UrlRedirectHandler(AppDbContext dbContext, IMemoryCache cache, ILogger<UrlRedirectHandler> logger)
    {
        this.dbContext = dbContext;
        this.cache = cache;
        this.logger = logger;
    }

    public Task<Results<RedirectHttpResult, BadRequest, NotFound>> Handle(UrlRedirectRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Code))
        {
            Results<RedirectHttpResult, BadRequest, NotFound> response = TypedResults.BadRequest();
            return Task.FromResult(response);
        }

        if (cache.TryGetValue(request.Code, out string? cachedUrl))
        {
            logger.LogInformation("Redirecting from cache");
            Results<RedirectHttpResult, BadRequest, NotFound> redirectFromCache = TypedResults.Redirect(cachedUrl!);
            return Task.FromResult(redirectFromCache);
        }

        var savedCode = dbContext.ShortnedUrls.FirstOrDefault(x => x.Code == request.Code);
        if (savedCode == null)
        {
            logger.LogWarning("Code not found");
            Results<RedirectHttpResult, BadRequest, NotFound> response = TypedResults.NotFound();
            return Task.FromResult(response);
        }

        cache.Set(request.Code, savedCode.OriginalUrl, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
        });
        logger.LogInformation("Redirecting from database");
        Results<RedirectHttpResult, BadRequest, NotFound> redirect =  TypedResults.Redirect(savedCode.OriginalUrl);
        return Task.FromResult(redirect);
    }
}
