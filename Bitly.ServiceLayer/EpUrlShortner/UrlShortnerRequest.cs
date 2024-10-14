using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bitly.UrlServices;

public class UrlShortnerRequest : IRequest<Results<Ok<ShortUrlResponse>, BadRequest>>
{
    public string Url { get; set; } = string.Empty;
}
