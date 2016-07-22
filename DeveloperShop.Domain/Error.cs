using DeveloperShop.Domain.ErrorHandling;

namespace DeveloperShop.Domain
{
    public static class Error
    {
        public static DeveloperShopException DeveloperNull()
        {
            return new DeveloperShopException("Developer should not be null", ErrorType.DeveloperNull);
        }

        public static DeveloperShopException InvalidAmountOfHours()
        {
            return new DeveloperShopException("Amount of Hours should be greather than zero", ErrorType.InvalidAmountOfHours);
        }

        public static DeveloperShopException ItemAlreadyAddedInCart()
        {
            return new DeveloperShopException("The item was already added in the cart", ErrorType.ItemAlreadyAdded);
        }

        public static DeveloperShopException ItemNotPresentInCart()
        {
            return new DeveloperShopException("The item was not added in the cart", ErrorType.ItemNotPresentInCart);
        }

        public static DeveloperShopException CouponNull()
        {
            return new DeveloperShopException("Discount Coupon should not be null", ErrorType.CouponNull);
        }
    }
}
