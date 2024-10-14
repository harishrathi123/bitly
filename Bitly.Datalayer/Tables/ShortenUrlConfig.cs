using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bitly.Database;

public class ShortenUrlConfig: IEntityTypeConfiguration<ShortnedUrl>
{
    public void Configure(EntityTypeBuilder<ShortnedUrl> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.OriginalUrl).IsRequired();
        builder.Property(e => e.ShortUrl).IsRequired();
        builder.Property(e => e.Code).IsRequired();
        builder.Property(e => e.CreatedOn).IsRequired();

        builder.HasIndex(e => e.Code).IsUnique();
        builder.HasIndex(e => e.ShortUrl).IsUnique();
        builder.HasIndex(e => e.OriginalUrl).IsUnique();
    }
}