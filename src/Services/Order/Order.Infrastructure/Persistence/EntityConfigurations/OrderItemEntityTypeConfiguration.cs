namespace Order.Infrastructure.Persistence.EntityConfigurations;

internal class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // 省略HasKey配置，EF Core会根据约定将Id属性作为主键
        builder.Property(oi => oi.Id)
            .HasConversion(orderItemId => orderItemId.Value, dbId => OrderItemId.Create(dbId));
        
        builder.Property(oi => oi.Quantity).IsRequired();
        
        builder.Property(oi => oi.Price).IsRequired();
        
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // 有关联OrderItem时拒绝删除Product，抛出异常

        // EF Core会根据隐式外键+导航属性的约定，自动将OrderId作为外键列添加到OrderItem表中，无需显式配置
    }
}
