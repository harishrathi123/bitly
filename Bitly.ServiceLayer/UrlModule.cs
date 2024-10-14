using Bitly.UrlServices;
using Carter;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Bitly;

public class UrlModule: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/shorten", async (IMediator mediator, UrlShortnerRequest request) =>
        {
            return await mediator.Send(request);
        });

        app.MapGet("/api/{code}", async (IMediator mediator, string code) =>
        {
            return await mediator.Send(new UrlRedirectRequest { Code = code });
        });
    }
}
