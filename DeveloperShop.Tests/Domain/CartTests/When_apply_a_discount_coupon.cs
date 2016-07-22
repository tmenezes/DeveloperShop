using DeveloperShop.Domain;
using DeveloperShop.Domain.ErrorHandling;
using NUnit.Framework;

namespace DeveloperShop.Tests.Domain.CartTests
{
    [TestFixture]
    public abstract class When_apply_a_discount_coupon
    {
        [TestFixture]
        public class Given_null_coupon : When_apply_a_discount_coupon
        {
            [Test]
            public void Should_thrown_an_exception()
            {
                // arrange
                var cart = StubFactory.GetCartWithItems(1);
                DiscountCoupon coupon = null;

                // act
                TestDelegate act = () => cart.ApplyDiscount(coupon);

                // assert
                var ex = Assert.Throws<DeveloperShopException>(act);
                Assert.AreEqual(ErrorType.CouponNull, ex.ErrorType);
            }
        }

        [TestFixture]
        public class Given_cart_without_coupon : When_apply_a_discount_coupon
        {
            // arrange
            static readonly Developer developer = StubFactory.GetDeveloper(100);
            static readonly Cart cart = StubFactory.GetCart(developer, hours: 1);
            static readonly DiscountCoupon coupon = StubFactory.GetCoupon(discoutPercentage: 0.1);

            [Test]
            public void Cart_discount_should_be_10percent_of_total_items()
            {
                // act
                cart.ApplyDiscount(coupon);

                // assert
                Assert.AreEqual(10, cart.Discount);
            }

            [Test]
            public void Cart_total_should_be_total_items_minus_discount()
            {
                // act
                cart.ApplyDiscount(coupon);

                // assert
                Assert.AreEqual(90, cart.CartPrice);
            }

            [Test]
            public void Cart_coupon_property_should_be_set()
            {
                // act
                cart.ApplyDiscount(coupon);

                // assert
                Assert.IsNotNull(cart.Coupon);
                Assert.AreEqual(coupon, cart.Coupon);
            }
        }

        [TestFixture]
        public class Given_cart_with_lower_coupon_applied : When_apply_a_discount_coupon
        {
            Cart cart;
            DiscountCoupon coupon;

            private void Arrange()
            {
                var developer = StubFactory.GetDeveloper(100);
                cart = StubFactory.GetCart(developer, hours: 1);
                coupon = StubFactory.GetCoupon(discoutPercentage: 0.1); // 10%

                cart.ApplyDiscount(StubFactory.GetCoupon(discoutPercentage: 0.05)); // cart with 5%
            }

            [Test]
            public void Cart_discount_should_be_10percent_of_total_items()
            {
                Arrange();

                // act
                cart.ApplyDiscount(coupon);

                // assert
                Assert.AreEqual(10, cart.Discount);
            }

            [Test]
            public void Cart_total_should_be_total_items_minus_discount()
            {
                Arrange();

                // act
                cart.ApplyDiscount(coupon);

                // assert
                Assert.AreEqual(90, cart.CartPrice);
            }

            [Test]
            public void Cart_coupon_property_should_be_set()
            {
                Arrange();

                // act
                cart.ApplyDiscount(coupon);

                // assert
                Assert.IsNotNull(cart.Coupon);
                Assert.AreEqual(coupon, cart.Coupon);
            }
        }

        [TestFixture]
        public class Given_cart_with_greater_coupon_applied : When_apply_a_discount_coupon
        {
            Cart cart;
            DiscountCoupon coupon;

            private void Arrange()
            {
                var developer = StubFactory.GetDeveloper(100);
                cart = StubFactory.GetCart(developer, hours: 1);
                coupon = StubFactory.GetCoupon(discoutPercentage: 0.1); // 10%

                cart.ApplyDiscount(StubFactory.GetCoupon(discoutPercentage: 0.2)); // cart with 20%
            }

            [Test]
            public void Cart_discount_should_be_10percent_of_total_items()
            {
                Arrange();

                // act
                cart.ApplyDiscount(coupon);

                // assert
                Assert.AreEqual(20, cart.Discount);
            }

            [Test]
            public void Cart_total_should_be_total_items_minus_discount()
            {
                Arrange();

                // act
                cart.ApplyDiscount(coupon);

                // assert
                Assert.AreEqual(80, cart.CartPrice);
            }

            [Test]
            public void Cart_coupon_property_should_be_set()
            {
                Arrange();

                // act
                cart.ApplyDiscount(coupon);

                // assert
                Assert.IsNotNull(cart.Coupon);
                Assert.AreNotEqual(coupon, cart.Coupon);
            }
        }
    }
}
