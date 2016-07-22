using System;
using DeveloperShop.Domain;

namespace DeveloperShop.Tests
{
    public static class StubFactory
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        // developers
        public static Developer GetDeveloper()
        {
            return GetDeveloper(150);
        }

        public static Developer GetDeveloper(decimal price)
        {
            return new Developer(Random.Next(), "_tmenezes_")
            {
                ImageUrl = "https://avatars.githubusercontent.com/u/91613?v=3",
                Followers = 20,
                Repositories = 5,
                Following = 10,
                Price = price
            };
        }

        // carts
        public static Cart GetCart(Developer dev, int hours)
        {
            var cart = new Cart();
            cart.AddItem(dev, hours);
            return cart;
        }

        public static Cart GetCartWithItems(int itemsQuantity)
        {
            var cart = new Cart();
            for (var i = 0; i < itemsQuantity; i++)
            {
                cart.AddItem(GetDeveloper(GetRandonDecimalNumber()), Random.Next());
            }
            return cart;
        }

        public static DiscountCoupon GetCoupon(double discoutPercentage)
        {
            return new DiscountCoupon("SHIPIT")
            {
                Description = "A 10% discount coupon",
                DiscountPercentage = discoutPercentage,
            };
        }


        // private
        private static decimal GetRandonDecimalNumber()
        {
            return (decimal)Random.NextDouble() * Random.Next();
        }
    }
}