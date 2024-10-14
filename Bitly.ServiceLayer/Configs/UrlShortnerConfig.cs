using System.ComponentModel.DataAnnotations;

namespace Bitly.UrlServices;

public class UrlShortnerConfig
{
    public static string SectionName => "UrlShortner";

    [Required, MaxLength(256), MinLength(32)]
    public string Charset { get; init; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    [Required, Range(6, 32)]
    public int CodeLength { get; init; } = 10;

    [Url] //config-option not used currently, just showing an option to use it
    public string SiteUrl { get; init; } = "http://bit.ly/";
}
