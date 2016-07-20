namespace DeveloperShop.Domain.Repository
{
    public interface IDiscountCouponRepository
    {
        DiscountCoupon GetCouponByKey(string key);
    }
}