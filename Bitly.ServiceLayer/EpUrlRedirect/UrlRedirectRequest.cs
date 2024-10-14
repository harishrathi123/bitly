using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Bitly.UrlServices;

public class UrlRedirectRequest : IRequest<Results<RedirectHttpResult, BadRequest, NotFound>>
{
    public string Code { get; set; } = string.Empty;
}
