using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using market.Core.Enums;
using market.Core.Models.Identity;

namespace market.DataAccess.Configurations.Identity;

public sealed class OtpCodeConfiguration : IEntityTypeConfiguration<OtpCode>
{
    public void Configure(EntityTypeBuilder<OtpCode> builder)
    {
        builder.ToTable("OtpCodes");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Target).IsRequired().HasMaxLength(256);
        builder.Property(x => x.Purpose).HasConversion<int>().IsRequired();
        builder.Property(x => x.Channel).HasConversion<int>().IsRequired();
        builder.Property(x => x.CodeHash).IsRequired().HasMaxLength(128);
        builder.Property(x => x.CreatedByIp).HasMaxLength(64);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.Property(x => x.ExpiresAtUtc).IsRequired();

        builder.HasIndex(x => new { x.Target, x.Purpose });
        builder.HasIndex(x => x.ExpiresAtUtc);

        builder.HasOne(x => x.User)
            .WithMany(x => x.OtpCodes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}