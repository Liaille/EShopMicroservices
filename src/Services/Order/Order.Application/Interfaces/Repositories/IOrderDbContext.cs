namespace Order.Application.Interfaces.Repositories;

public interface IOrderDbContext
{
    DbSet<Customer> Customers { get; }

    DbSet<PaymentMethod> PaymentMethods { get; }

    DbSet<CardType> CardTypes { get; }

    DbSet<Product> Products { get; }

    DbSet<Domain.AggregateModels.OrderAggregate.Order> Orders { get; }

    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
