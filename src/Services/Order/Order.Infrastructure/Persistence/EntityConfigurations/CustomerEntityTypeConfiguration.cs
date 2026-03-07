namespace Order.Infrastructure.Persistence.EntityConfigurations;

internal class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        // 配置属性转换器，将 CustomerId 转换为 Guid 存储在数据库中
        builder.Property(c => c.Id)
            .HasConversion(customerId => customerId.Value, dbId => CustomerId.Create(dbId));
        
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        
        builder.Property(c => c.Email).HasMaxLength(255);
        // 配置 Email 字段为唯一索引，确保每个客户的邮箱地址唯一
        builder.HasIndex(c => c.Email).IsUnique();

        builder.HasMany(c => c.PaymentMethods).WithOne();
    }
}
