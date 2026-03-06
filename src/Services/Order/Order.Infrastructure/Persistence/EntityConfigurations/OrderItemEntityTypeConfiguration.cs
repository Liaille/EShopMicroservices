namespace Order.Infrastructure.Persistence.EntityConfigurations;

internal class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // 省略HasKey配置，EF Core会根据约定将Id属性作为主键
        // 使用Hi/Lo(高低位)算法生成OrderItemId
        // 插入前先从数据库获取一个高位值，然后在内存中生成低位值，组合成完整的OrderItemId
        // 对DDD聚合内关联实体的Id生成非常合适，避免了分布式环境下的Id冲突问题，同时也减少了数据库访问次数，提高性能
        builder.Property(oi => oi.Id)
            .UseHiLo()
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
