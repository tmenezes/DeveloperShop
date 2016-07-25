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
    public abstract class When_apply_coupon
    {
        CartItemRequestData requestData;
        Mock<IDiscountCouponRepository> mockIDiscountCouponRepository;
        CartController controller;

        protected abstract void Arrange();

        [TestFixture]
        public class Given_inexistent_coupon : When_apply_coupon
        {
            protected override void Arrange()
            {
                requestData = new CartItemRequestData();
                mockIDiscountCouponRepository = new Mock<IDiscountCouponRepository>();
                controller = new SingleCartController(null, mockIDiscountCouponRepository.Object);
            }

            [Test]
            public void Should_return_status_code_bad_request()
            {
                Arrange();

                // act
                var httpResult = controller.ApplyDiscount(requestData);

                // assert
                Assert.IsInstanceOf<BadRequestErrorMessageResult>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.ApplyDiscount(requestData);

                mockIDiscountCouponRepository.Verify(mock => mock.GetCouponByKey(requestData.CouponKey), Times.Once);
            }
        }

        [TestFixture]
        public class Given_perfect_scenario : When_apply_coupon
        {
            protected override void Arrange()
            {
                requestData = new CartItemRequestData() { CouponKey = "SHIPIT" };

                mockIDiscountCouponRepository = new Mock<IDiscountCouponRepository>();
                mockIDiscountCouponRepository.Setup(mock => mock.GetCouponByKey(requestData.CouponKey))
                                             .Returns(StubFactory.GetCoupon(0.1));

                controller = new SingleCartController(null, mockIDiscountCouponRepository.Object);
            }

            [Test]
            public void Should_return_status_code_Ok()
            {
                Arrange();

                // act
                var httpResult = controller.ApplyDiscount(requestData);

                // assert
                Assert.IsInstanceOf<OkNegotiatedContentResult<Cart>>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.ApplyDiscount(requestData);

                mockIDiscountCouponRepository.Verify(mock => mock.GetCouponByKey(requestData.CouponKey), Times.Once);
            }
        }
    }
}