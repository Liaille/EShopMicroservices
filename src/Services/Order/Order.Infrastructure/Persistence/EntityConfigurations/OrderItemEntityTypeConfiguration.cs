namespace Order.Infrastructure.Persistence.EntityConfigurations;

internal class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).HasConversion(orderItemId => orderItemId.Value, dbId => OrderItemId.Create(dbId));
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.Price).IsRequired();
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict); // 有关联OrderItem时拒绝删除Product，抛出异常
        // OrderId的关联关系在Order中配置
    }
}
