namespace Order.Infrastructure.Persistence.DbContexts;

public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<CardType> CardTypes => Set<CardType>();

    public DbSet<Domain.AggregateModels.OrderAggregate.Order> Orders => Set<Domain.AggregateModels.OrderAggregate.Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 自动扫描OrderDbContext所在程序集的所有IEntityTypeConfiguration<T>实现类
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}
