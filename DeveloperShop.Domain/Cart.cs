using System;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperShop.Domain
{
    public class Cart
    {
        private readonly IList<CartItem> _items;

        public string Id { get; set; }
        public IEnumerable<CartItem> Items => _items;
        public DiscountCoupon Coupon { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal CartPrice { get; set; }

        public Cart()
        {
            Id = Guid.NewGuid().ToString();
            _items = new List<CartItem>();
        }


        public void AddItem(Developer developer, int amountOfHours)
        {
            if (developer == null)
                throw Error.DeveloperNull();

            if (amountOfHours <= 0)
                throw Error.InvalidAmountOfHours();

            var alreadyAdded = _items.Any(d => d.Developer.Id == developer.Id);
            if (alreadyAdded)
                throw Error.ItemAlreadyAddedInCart();

            _items.Add(new CartItem(developer, amountOfHours));
            UpdateCartPrices();
        }

        public void RemoveItem(Developer developer)
        {
            if (developer == null)
                throw Error.DeveloperNull();

            var developerAdded = _items.FirstOrDefault(d => d.Developer.Id == developer.Id);
            if (developerAdded == null)
                throw Error.ItemAlreadyAddedInCart();

            _items.Remove(developerAdded);
            UpdateCartPrices();
        }

        public void ApplyDiscount(DiscountCoupon coupon)
        {
            var actualCouponPercentage = this.Coupon?.DiscountPercentage ?? 0d;

            if (coupon.DiscountPercentage > actualCouponPercentage)
            {
                Coupon = coupon;
                UpdateCartPrices();
            }
        }


        private void UpdateCartPrices()
        {
            TotalPrice = Items.Sum(i => i.TotalPrice);
            Discount = TotalPrice * (decimal)(Coupon?.DiscountPercentage ?? 0);
            CartPrice = TotalPrice - Discount;
        }
    }
}