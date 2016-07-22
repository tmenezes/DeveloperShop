using System;
using System.Linq;
using DeveloperShop.Domain;
using DeveloperShop.Domain.ErrorHandling;
using NUnit.Framework;

namespace DeveloperShop.Tests.Domain.CartTests
{
    [TestFixture]
    public class When_remove_item_from_cart
    {
        [TestFixture]
        public class Given_null_developer : When_add_item_to_cart
        {
            [Test]
            public void Should_thrown_an_exception()
            {
                // arrange
                var cart = new Cart();
                Developer developer = null;

                // act
                TestDelegate act = () => cart.RemoveItem(developer);

                // assert
                var ex = Assert.Throws<DeveloperShopException>(act);
                Assert.AreEqual(ErrorType.DeveloperNull, ex.ErrorType);
            }
        }

        [TestFixture]
        public class Given_a_not_added_item : When_add_item_to_cart
        {
            [Test]
            public void Should_thrown_an_exception()
            {
                var cart = StubFactory.GetCartWithItems(10);
                var developer = StubFactory.GetDeveloper();

                // act
                TestDelegate act = () => cart.RemoveItem(developer);

                // assert
                var ex = Assert.Throws<DeveloperShopException>(act);
                Assert.AreEqual(ErrorType.ItemNotPresentInCart, ex.ErrorType);
            }
        }

        [TestFixture]
        public class Given_a_added_item : When_add_item_to_cart
        {
            Cart cart;
            CartItem carItem;
            decimal expectedTotalCartValue;

            private void Arrange()
            {
                cart = StubFactory.GetCartWithItems(10);
                carItem = cart.Items.First();

                expectedTotalCartValue = cart.CartPrice - carItem.TotalPrice;
            }

            [Test]
            public void Cart_total_value_should_be_subtracted()
            {
                Arrange();

                // act
                cart.RemoveItem(carItem.Developer);

                // assert
                Assert.AreEqual(expectedTotalCartValue.RoundIt(4), cart.CartPrice.RoundIt(4));
            }

            [Test]
            public void Cart_total_items_value_should_be_subtracted()
            {
                Arrange();

                // act
                cart.RemoveItem(carItem.Developer);

                // assert
                Assert.AreEqual(expectedTotalCartValue.RoundIt(4), cart.TotalPrice.RoundIt(4));
            }
        }
    }

    static class DecimalExtensions
    {
        public static decimal RoundIt(this decimal value, int precision)
        {
            return Decimal.Round(value, precision);
        }
    }
}
