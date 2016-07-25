using System;
using System.Linq;
using System.Web.Http;
using DeveloperShop.Domain;
using DeveloperShop.Domain.ErrorHandling;
using DeveloperShop.Web.Models;

namespace DeveloperShop.Web.Controllers
{
    public class DevShopApiController : ApiController
    {
        protected virtual string UserCartId
        {
            get
            {
                return Request.Headers.Contains("auth_cart_id")
                    ? Request.Headers.GetValues("auth_cart_id").First()
                    : "new key";
            }
        }


        protected Cart GetCart()
        {
            return CartHolder.GetCart(UserCartId);
        }

        protected bool TryDomainOperation(Action act, out IHttpActionResult httpActionResult)
        {
            try
            {
                act();
                httpActionResult = Ok();
                return true;
            }
            catch (DeveloperShopException ex)
            {
                httpActionResult = GetHttpResultFromDomainException(ex);
                return false;
            }
            catch (Exception ex)
            {
                httpActionResult = InternalServerError(ex);
                return false;
            }
        }

        protected IHttpActionResult GetHttpResultFromDomainException(DeveloperShopException ex)
        {
            switch (ex.ErrorType)
            {
                case ErrorType.DeveloperNull:
                case ErrorType.InvalidAmountOfHours:
                case ErrorType.CouponNull:
                case ErrorType.ItemNotPresentInCart:
                    return BadRequest(ex.Message);

                case ErrorType.ItemAlreadyAdded:
                    return Conflict();

                default:
                    return InternalServerError(ex);
            }
        }
    }
}