namespace Order.Infrastructure.Persistence.EntityConfigurations;

internal class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        // 省略HasKey配置，EF Core会根据约定将Id属性作为主键
        builder.Property(p => p.Id)
            .UseHiLo()
            .HasConversion(paymentMethodId => paymentMethodId.Value, dbId => PaymentMethodId.Create(dbId));
        // 隐式配置CustomerId属性为外键
        builder.Property<Guid>("CustomerId").IsRequired();

        builder.Property(p => p.CardTypeId).IsRequired();

        builder.Property(p => p.CardNumber).HasMaxLength(4).IsRequired();

        builder.Property(p => p.CardHolderName).HasMaxLength(200).IsRequired();

        builder.Property(p => p.Expiration).IsRequired();
        // 关联CardType配置(跨聚合关联)
        builder.HasOne(p => p.CardType)
            .WithMany()
            .HasForeignKey(p => p.CardTypeId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
        // 按CustomerId+卡号后4位+卡类型+有效期建立唯一索引(防重复绑定)
        builder.HasIndex(["CustomerId", nameof(PaymentMethod.CardNumber), nameof(PaymentMethod.CardTypeId)], nameof(PaymentMethod.Expiration))
            .IsUnique()
            .HasDatabaseName("Idx_PaymentMethods_CustomerCardUnique");
    }
}
