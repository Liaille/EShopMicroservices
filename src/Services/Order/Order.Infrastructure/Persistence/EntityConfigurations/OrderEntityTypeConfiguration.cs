namespace Order.Infrastructure.Persistence.EntityConfigurations;

internal class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregateModels.OrderAggregate.Order>
{
    public void Configure(EntityTypeBuilder<Domain.AggregateModels.OrderAggregate.Order> builder)
    {
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id).HasConversion(orderId => orderId.Value, dbId => OrderId.Create(dbId));
        
        builder.Property(o => o.CustomerId).IsRequired();

        builder.Property(o => o.Status)
            .HasDefaultValue(OrderStatus.Submitted)
            .HasConversion(s => s.ToString(), dbStatus => Enum.Parse<OrderStatus>(dbStatus));

        builder.Property(o => o.TotalPrice);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<PaymentMethod>()
            .WithMany()
            .HasForeignKey(o => o.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ComplexProperty(o => o.OrderName, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value)
                .HasColumnName(nameof(Domain.AggregateModels.OrderAggregate.Order.OrderName))
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.ConfigureAddressComplexProperty(o => o.ShippingAddress);

        builder.ConfigureAddressComplexProperty(o => o.BillingAddress);


    }
}
