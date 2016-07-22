using DeveloperShop.Domain;
using DeveloperShop.Domain.ErrorHandling;
using NUnit.Framework;

namespace DeveloperShop.Tests.Domain.CartTests
{
    public abstract class When_add_item_to_cart
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
                int hours = 8;

                // act
                TestDelegate act = () => cart.AddItem(developer, hours);

                // assert
                var ex = Assert.Throws<DeveloperShopException>(act);
                Assert.AreEqual(ErrorType.DeveloperNull, ex.ErrorType);
            }
        }

        [TestFixture]
        public class Given_negative_amount_of_hours : When_add_item_to_cart
        {
            [Test]
            public void Should_thrown_an_exception()
            {
                // arrange
                var cart = new Cart();
                var developer = StubFactory.GetDeveloper();
                var hours = -1;

                // act
                TestDelegate act = () => cart.AddItem(developer, hours);

                // assert
                var ex = Assert.Throws<DeveloperShopException>(act);
                Assert.AreEqual(ErrorType.InvalidAmountOfHours, ex.ErrorType);
            }
        }

        [TestFixture]
        public class Given_zero_amount_of_hours : When_add_item_to_cart
        {
            [Test]
            public void Should_thrown_an_exception()
            {
                // arrange
                var cart = new Cart();
                var developer = StubFactory.GetDeveloper();
                var hours = 0;

                // act
                TestDelegate act = () => cart.AddItem(developer, hours);

                // assert
                var ex = Assert.Throws<DeveloperShopException>(act);
                Assert.AreEqual(ErrorType.InvalidAmountOfHours, ex.ErrorType);
            }
        }

        [TestFixture]
        public class Given_developer_price_and_an_hour : When_add_item_to_cart
        {
            [TestCase(50, 1, 50)]
            [TestCase(50, 2, 100)]
            [TestCase(50, 10, 500)]
            public void Itens_total_price_should_be_calculated_correctly(decimal devPrice, int hours, decimal expectedPrice)
            {
                var cart = new Cart();
                var developer = StubFactory.GetDeveloper(devPrice);

                // act
                cart.AddItem(developer, hours);

                // assert
                Assert.AreEqual(expectedPrice, cart.TotalPrice);
            }

            [TestCase(50, 1, 50)]
            [TestCase(50, 2, 100)]
            [TestCase(50, 10, 500)]
            public void Cart_price_should_be_calculated_correctly(decimal devPrice, int hours, decimal expectedPrice)
            {
                var cart = new Cart();
                var developer = StubFactory.GetDeveloper(devPrice);

                // act
                cart.AddItem(developer, hours);

                // assert
                Assert.AreEqual(expectedPrice, cart.CartPrice);
            }
        }

        [TestFixture]
        public class Given_cart_with_already_one_item_added : When_add_item_to_cart
        {
            [TestCase(100, 50, 1, 150)]
            [TestCase(100, 50, 2, 200)]
            [TestCase(100, 50, 10, 600)]
            public void Itens_total_price_should_be_calculated_correctly(decimal existentCartPrice, decimal devPrice, int hours, decimal expectedPrice)
            {
                var cart = new Cart();
                cart.AddItem(StubFactory.GetDeveloper(existentCartPrice), 1);

                var developer = StubFactory.GetDeveloper(devPrice);

                // act
                cart.AddItem(developer, hours);

                // assert
                Assert.AreEqual(expectedPrice, cart.TotalPrice);
            }

            [TestCase(100, 50, 1, 150)]
            [TestCase(100, 50, 2, 200)]
            [TestCase(100, 50, 10, 600)]
            public void Cart_price_should_be_calculated_correctly(decimal existentCartPrice, decimal devPrice, int hours, decimal expectedPrice)
            {
                var cart = new Cart();
                cart.AddItem(StubFactory.GetDeveloper(existentCartPrice), 1);

                var developer = StubFactory.GetDeveloper(devPrice);

                // act
                cart.AddItem(developer, hours);

                // assert
                Assert.AreEqual(expectedPrice, cart.CartPrice);
            }
        }

        [TestFixture]
        public class Given_same_item_twice : When_add_item_to_cart
        {
            [Test]
            public void Should_thrown_an_exception()
            {
                var cart = new Cart();
                var hours = 8;
                var developer = StubFactory.GetDeveloper();

                // act
                TestDelegate act = () => cart.AddItem(developer, hours);
                act.Invoke(); // add item once

                // assert
                var ex = Assert.Throws<DeveloperShopException>(act); // twice
                Assert.AreEqual(ErrorType.ItemAlreadyAdded, ex.ErrorType);
            }
        }
    }
}
