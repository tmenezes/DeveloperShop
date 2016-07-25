using System.Web.Http.Results;
using DeveloperShop.Domain;
using DeveloperShop.Domain.Repository;
using DeveloperShop.Web.Controllers;
using DeveloperShop.Web.Models;
using Moq;
using NUnit.Framework;

namespace DeveloperShop.Tests.Api.CartControllerTests
{
    [TestFixture]
    public abstract class When_add_item_in_cart
    {
        CartItemRequestData requestData;
        Developer dev;
        Mock<IDeveloperRepository> mockIDeveloperRepository;
        CartController controller;

        protected abstract void Arrange();

        [TestFixture]
        public class Given_inexistent_developer : When_add_item_in_cart
        {
            protected override void Arrange()
            {
                requestData = new CartItemRequestData();
                mockIDeveloperRepository = new Mock<IDeveloperRepository>();
                controller = new SingleCartController(mockIDeveloperRepository.Object, null);
            }

            [Test]
            public void Should_return_status_code_bad_request()
            {
                Arrange();

                // act
                var httpResult = controller.Post(requestData);

                // assert
                Assert.IsInstanceOf<BadRequestErrorMessageResult>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.Post(requestData);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(requestData.DeveloperId), Times.Once);
            }
        }

        [TestFixture]
        public class Given_invalid_amount_of_hours : When_add_item_in_cart
        {
            protected override void Arrange()
            {
                requestData = new CartItemRequestData() { AmountOfHours = -1 };
                dev = StubFactory.GetDeveloper();

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();
                mockIDeveloperRepository.Setup(mock => mock.GetDeveloper(requestData.DeveloperId))
                                        .Returns(dev);

                controller = new SingleCartController(mockIDeveloperRepository.Object, null);
            }

            [Test]
            public void Should_return_status_code_bad_request()
            {
                Arrange();

                // act
                var httpResult = controller.Post(requestData);

                // assert
                Assert.IsInstanceOf<BadRequestErrorMessageResult>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.Post(requestData);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(requestData.DeveloperId), Times.Once);
            }
        }

        [TestFixture]
        public class Given_already_added_developer : When_add_item_in_cart
        {
            protected override void Arrange()
            {
                requestData = new CartItemRequestData() { AmountOfHours = 8 };
                dev = StubFactory.GetDeveloper();

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();
                mockIDeveloperRepository.Setup(mock => mock.GetDeveloper(requestData.DeveloperId))
                                        .Returns(dev);

                controller = new SingleCartController(mockIDeveloperRepository.Object, null);
                (controller as SingleCartController).CurrentCart.AddItem(dev, 8); // add item 1x
            }

            [Test]
            public void Should_return_status_code_conflict()
            {
                Arrange();

                // act
                var httpResult = controller.Post(requestData); // adding item for the second time

                // assert
                Assert.IsInstanceOf<ConflictResult>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.Post(requestData);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(requestData.DeveloperId), Times.Once);
            }
        }

        [TestFixture]
        public class Given_perfect_scenario : When_add_item_in_cart
        {
            protected override void Arrange()
            {
                requestData = new CartItemRequestData() { AmountOfHours = 8 };
                dev = StubFactory.GetDeveloper();

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();
                mockIDeveloperRepository.Setup(mock => mock.GetDeveloper(requestData.DeveloperId))
                                        .Returns(dev);

                controller = new SingleCartController(mockIDeveloperRepository.Object, null);
            }

            [Test]
            public void Should_return_status_code_created()
            {
                Arrange();

                // act
                var httpResult = controller.Post(requestData); // adding item for the second time

                // assert
                Assert.IsInstanceOf<CreatedNegotiatedContentResult<Cart>>(httpResult);
            }

            [Test]
            public void Created_url_should_contains_developer_id()
            {
                Arrange();

                // act
                var httpResult = controller.Post(requestData); // adding item for the second time
                var createdUrl = (httpResult as CreatedNegotiatedContentResult<Cart>).Location.OriginalString;

                // assert
                Assert.IsTrue(createdUrl.Contains(dev.Id.ToString()));
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.Post(requestData);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(requestData.DeveloperId), Times.Once);
            }
        }
    }
}
