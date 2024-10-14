using FluentValidation;
using MediatR;

namespace Bitly.Middleware;

public class FluentValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        // if not validators, just continue
        if (!validators.Any())
            return await next();

        // if no validation failures, just continue
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        if (Array.TrueForAll(validationResults, x => x.IsValid))
            return await next();

        // throw validation exception
        var failures = validationResults.SelectMany(x => x.Errors).ToList();
        throw new ValidationException(failures);
    }
}