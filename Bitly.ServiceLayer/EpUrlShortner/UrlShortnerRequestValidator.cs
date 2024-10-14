using FluentValidation;

namespace Bitly.UrlServices;

public class UrlShortnerRequestValidator : AbstractValidator<UrlShortnerRequest>
{
    public UrlShortnerRequestValidator()
    {
        RuleFor(x => x.Url).NotEmpty().WithMessage("Url is required");

        RuleFor(x => x.Url).Must(x => Uri.TryCreate(x, UriKind.Absolute, out _)).WithMessage("Url is not valid");
    }
}
