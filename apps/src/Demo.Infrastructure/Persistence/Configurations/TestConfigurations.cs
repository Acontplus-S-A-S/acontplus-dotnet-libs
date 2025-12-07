namespace Demo.Infrastructure.Persistence.Configurations;

public class WhatsAppUsageConfiguration : BaseEntityTypeConfiguration<WhatsAppUsage>
{
    public override void Configure(EntityTypeBuilder<WhatsAppUsage> builder)
    {
        base.Configure(builder);

        // builder
        //     .HasOne<Company>()
        //     .WithMany(c => c.WhatsAppUsages)
        //     .HasForeignKey(x => x.CompanyId)
        //     .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasIndex(x => x.CompanyId)
            .HasDatabaseName("UX_WhatsAppUsage_CompanyId")
            .IsUnique();

        builder
            .Property(x => x.Used)
            .HasDefaultValue(0);

        builder
            .Property(x => x.Limit)
            .HasDefaultValue(0);

        builder
            .Property(x => x.Unlimited)
            .HasDefaultValue(false);

        builder
            .Property(x => x.StartDate)
            .HasDefaultValueSql("SYSUTCDATETIME()");

        builder.ToTable("WhatsAppUsage", "Config");
    }
}

