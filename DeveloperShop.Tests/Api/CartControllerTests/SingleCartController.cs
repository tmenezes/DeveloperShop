using DeveloperShop.Domain;
using DeveloperShop.Domain.Repository;
using DeveloperShop.Web.Controllers;

namespace DeveloperShop.Tests.Api.CartControllerTests
{
    internal class SingleCartController : CartController
    {
        public SingleCartController(IDeveloperRepository developerRepository, IDiscountCouponRepository discountCouponRepository)
            : base(developerRepository, discountCouponRepository)
        {
        }


        protected override string UserCartId => "ALWAYS_SAME_ID";

        public Cart CurrentCart => GetCart();
    }
}
