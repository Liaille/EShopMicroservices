namespace Order.Infrastructure.Persistence.EntityConfigurations;

internal class PaymentMethodEntityTypeConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(paymentMethodId => paymentMethodId.Value, dbId => PaymentMethodId.Create(dbId));

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

        // 满足以下条件则仅在聚合根侧做完整的单向配置
        // 1.子实体有显式的外键属性
        // 2.仅通过聚合根操作子实体
        // 3.无需再子实体侧添加额外约束(如级联删除、外键非空)
        builder.HasOne<Customer>()
            .WithMany(c => c.PaymentMethods)
            .HasForeignKey("CustomerId")
            .IsRequired();

        // 按CustomerId+卡号后4位+卡类型+有效期建立唯一索引(防重复绑定)
        builder.HasIndex(["CustomerId", nameof(PaymentMethod.CardNumber), nameof(PaymentMethod.CardTypeId)], nameof(PaymentMethod.Expiration))
            .IsUnique()
            .HasDatabaseName("Idx_PaymentMethods_CustomerCardUnique");
    }
}
