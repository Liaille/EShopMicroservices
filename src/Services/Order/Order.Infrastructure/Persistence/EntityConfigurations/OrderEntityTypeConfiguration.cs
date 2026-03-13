using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            .HasConversion(new EnumToStringConverter<OrderStatus>())
            .HasMaxLength(50)
            .IsRequired();

        // TotalPrice是计算属性并且private set导致EF Core无法赋值，因此显式忽略该属性的映射
        builder.Ignore(o => o.TotalPrice);

        builder.HasOne<Customer>()
            .WithMany()
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne<PaymentMethod>()
            .WithMany()
            .HasForeignKey(o => o.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ComplexProperty(o => o.OrderName, nameBuilder =>
        {
            nameBuilder.Property(n => n.Value)
                .HasColumnName("OrderName")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.ConfigureAddressComplexProperty(o => o.ShippingAddress);

        builder.ConfigureAddressComplexProperty(o => o.BillingAddress);
    }
}
