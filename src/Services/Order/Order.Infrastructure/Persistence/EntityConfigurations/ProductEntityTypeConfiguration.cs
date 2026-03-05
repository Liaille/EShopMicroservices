namespace Order.Infrastructure.Persistence.EntityConfigurations;

internal class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        // 配置ProductId与数据库中的Guid类型之间的转换
        builder.Property(p => p.Id).HasConversion(productId => productId.Value, dbId => ProductId.Create(dbId));
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
    }
}
