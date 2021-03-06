﻿using System.Collections.Generic;
using System.Linq;

namespace DeveloperShop.Domain.Repository
{
    public class DiscountCouponRepository : IDiscountCouponRepository
    {
        private static readonly IList<DiscountCoupon> _coupons;

        static DiscountCouponRepository()
        {
            _coupons = new List<DiscountCoupon>()
            {
                new DiscountCoupon("SHIPIT")
                {
                    Description = "A 10% discount coupon",
                    DiscountPercentage = 0.1,
                }
            };
        }


        public DiscountCoupon GetCouponByKey(string key)
        {
            return _coupons.FirstOrDefault(c => c.Key == key);
        }
    }
}
