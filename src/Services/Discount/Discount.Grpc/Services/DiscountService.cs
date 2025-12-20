using Discount.Grpc.Data;
using Discount.Grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.Grpc.Services;
public class DiscountService(ILogger<DiscountService> logger, DiscountDbContext dbContext) : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName);

        coupon ??= new Coupon { ProductName = "No Discount", Amount = 0, Description = "No Discount Description." };
        if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Discount is retrieved for ProductName : {ProductName}, Amount : {Amount}", coupon.ProductName, coupon.Amount);

        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        Coupon coupon = request.Coupon.Adapt<Coupon>() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));
        dbContext.Coupons.Add(coupon);
        await dbContext.SaveChangesAsync();

        if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Discount is successfully created. ProductName : {ProductName}", coupon.ProductName);

        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        Coupon coupon = request.Coupon.Adapt<Coupon>() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));
        dbContext.Coupons.Update(coupon);
        await dbContext.SaveChangesAsync();

        if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Discount is successfully updated. ProductName : {ProductName}", coupon.ProductName);

        var couponModel = coupon.Adapt<CouponModel>();
        return couponModel;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName) ?? throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} is not found."));
        dbContext.Coupons.Remove(coupon);
        int deleted = await dbContext.SaveChangesAsync();

        if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Discount is successfully deleted. ProductName : {ProductName}", request.ProductName);

        var response = new DeleteDiscountResponse
        {
            Success = deleted > 0
        };
        return response;
    }
}
