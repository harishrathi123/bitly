using Microsoft.Extensions.Options;

namespace Bitly.UrlServices;

public class CodeGeneration: ICodeGeneration
{
    private readonly UrlShortnerConfig options;
    private readonly Random Random = new Random();

    public CodeGeneration(IOptions<UrlShortnerConfig> options)
    {
        this.options = options.Value;
    }

    public string BaseUrl() => options.SiteUrl;

    public string GenerateCode()
    {
        var repeater = Enumerable.Repeat(options.Charset, options.CodeLength);
        var result = repeater.Select(s => s[Random.Next(s.Length)]).ToArray();
        return new string(result);
    }
}
