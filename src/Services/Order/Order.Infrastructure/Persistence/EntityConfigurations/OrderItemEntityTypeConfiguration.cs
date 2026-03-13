namespace Order.Infrastructure.Persistence.EntityConfigurations;

internal class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id)
            .HasConversion(orderItemId => orderItemId.Value, dbId => OrderItemId.Create(dbId));
        
        builder.Property(oi => oi.Quantity).IsRequired();
        
        builder.Property(oi => oi.Price).IsRequired();

        builder.Property(oi => oi.ProductId).HasConversion(id => id.Value, dbId => ProductId.Create(dbId));
        
        builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Domain.AggregateModels.OrderAggregate.Order>()
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .IsRequired();
    }
}
