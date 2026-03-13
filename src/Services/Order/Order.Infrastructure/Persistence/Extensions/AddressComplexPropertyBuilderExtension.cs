using System.Linq.Expressions;

namespace Order.Infrastructure.Persistence.Extensions;

public static class AddressComplexPropertyBuilderExtension
{
    /// <summary>
    /// 为任意实体的Address属性配置ComplexProperty规则
    /// </summary>
    /// <typeparam name="TEntity">主实体类型</typeparam>
    /// <param name="builder">实体构建器</param>
    /// <param name="addressProperty">地址属性表达式</param>
    /// <param name="customConfigure">个性化配置(可选)</param>
    public static void ConfigureAddressComplexProperty<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Address>> addressProperty,
        Action<ComplexPropertyBuilder<Address>>? customConfigure = default)
        where TEntity : class
    {
        // 修复builder.ComplexProperty(addressProperty)时抛出CS8620警告
        var nullableAddressProperty = Expression.Lambda<Func<TEntity, Address?>>(
        Expression.Convert(addressProperty.Body, typeof(Address)),
        addressProperty.Parameters);
        var addressBuilder = builder.ComplexProperty(nullableAddressProperty);

        // 通用配置逻辑
        addressBuilder.Property(a => a.FirstName).HasMaxLength(50).IsRequired();

        addressBuilder.Property(a => a.LastName).HasMaxLength(50).IsRequired();

        addressBuilder.Property(a => a.Email).HasMaxLength(255);

        addressBuilder.Property(a => a.AddressLine).HasMaxLength(180).IsRequired();

        addressBuilder.Property(a => a.Country).HasMaxLength(50);

        addressBuilder.Property(a => a.State).HasMaxLength(50);

        addressBuilder.Property(a => a.ZipCode).HasMaxLength(10).IsRequired();

        customConfigure?.Invoke(addressBuilder);
    }
}
