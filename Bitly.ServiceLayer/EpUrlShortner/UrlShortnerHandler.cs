using Bitly.Database;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace Bitly.UrlServices;

public class UrlShortnerHandler : IRequestHandler<UrlShortnerRequest, Results<Ok<ShortUrlResponse>, BadRequest>>
{
    private readonly ICodeGeneration codeGeneration;
    private readonly AppDbContext dbContext;
    private readonly ILogger<UrlShortnerHandler> logger;
    private readonly HttpContext? httpContext;

    public UrlShortnerHandler(ICodeGeneration codeGeneration, 
        AppDbContext dbContext, 
        IHttpContextAccessor httpContextAccessor,
        ILogger<UrlShortnerHandler> logger)
    {
        this.codeGeneration = codeGeneration;
        this.dbContext = dbContext;
        this.logger = logger;
        this.httpContext = httpContextAccessor.HttpContext;
    }

    public Task<Results<Ok<ShortUrlResponse>, BadRequest>> Handle(UrlShortnerRequest request, CancellationToken cancellationToken)
    {
        var savedCode = dbContext.ShortnedUrls.FirstOrDefault(x => x.OriginalUrl == request.Url.ToLowerInvariant());
        if (savedCode != null)
        {
            logger.LogInformation("Code already exists");
            var response = ShortUrlResponse.FromEntity(savedCode);
            Results<Ok<ShortUrlResponse>, BadRequest> result = TypedResults.Ok(response);
            return Task.FromResult(result);
        }
        else
        {
            var code = GetUniqueCode();
            var response = ShortUrlResponse.Create(request, UrlPrefix(), code);
            dbContext.ShortnedUrls.Add(response.ToEntity());
            dbContext.SaveChanges();
            logger.LogInformation("New code created");
            Results<Ok<ShortUrlResponse>, BadRequest> result = TypedResults.Ok(response);
            return Task.FromResult(result);
        }
    }

    private string UrlPrefix() => $"{httpContext?.Request.Scheme}://{httpContext?.Request.Host}/";

    private string GetUniqueCode()
    {
        while (true)
        {
            var code = this.codeGeneration.GenerateCode();
            if (!dbContext.ShortnedUrls.Any(x => x.Code == code))
            {
                return code;
            }
        }
    }
}
