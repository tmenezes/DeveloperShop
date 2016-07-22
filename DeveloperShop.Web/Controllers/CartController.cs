using System;
using System.Linq;
using System.Web.Http;
using DeveloperShop.Domain;
using DeveloperShop.Domain.ErrorHandling;
using DeveloperShop.Domain.Repository;
using DeveloperShop.Web.Models;

namespace DeveloperShop.Web.Controllers
{
    public class CartController : ApiController
    {
        private const string USER_KEY = "KEY"; // TODO: implementar obtencao de token do usuario
        private readonly IDeveloperRepository _developerRepository;
        private readonly IDiscountCouponRepository _discountCouponRepository;

        public CartController()
            : this(new DeveloperRepository(), new DiscountCouponRepository())
        {
        }

        public CartController(IDeveloperRepository developerRepository, IDiscountCouponRepository discountCouponRepository)
        {
            _developerRepository = developerRepository;
            _discountCouponRepository = discountCouponRepository;
        }


        // GET: api/Cart
        public Cart Get()
        {
            return CartHolder.GetCart(USER_KEY);
        }

        // GET: api/Cart/:id
        public IHttpActionResult Get(int id)
        {
            var cart = CartHolder.GetCart(USER_KEY);
            var item = cart.Items.FirstOrDefault(i => i.Developer.Id == id);

            if (item == null)
            {
                return BadRequest("Cart item does not exists");
            }

            return Ok(item);
        }

        // POST: api/Cart
        public IHttpActionResult Post([FromBody]CartItemRequestData cartItemRequestData)
        {
            var cart = CartHolder.GetCart(USER_KEY);

            var developer = _developerRepository.GetDeveloper(cartItemRequestData.DeveloperId);
            if (developer == null)
            {
                return BadRequest("Developer does not exists");
            }

            IHttpActionResult httpResult;
            bool success = TryDomainOperation(() => cart.AddItem(developer, cartItemRequestData.AmountOfHours), out httpResult);
            if (!success)
                return httpResult;

            var createdUrl = $"api/cart/{developer.Id}";
            return Created(createdUrl, cart);
        }

        // DELETE: api/Cart/
        public IHttpActionResult Delete(int id)
        {
            var cart = CartHolder.GetCart(USER_KEY);

            var developer = _developerRepository.GetDeveloper(id);
            if (developer == null)
            {
                return BadRequest("Developer was not added on the cart");
            }

            IHttpActionResult httpResult;
            bool success = TryDomainOperation(() => cart.RemoveItem(developer), out httpResult);
            if (!success)
                return httpResult;

            return Ok(cart);
        }

        [HttpPut]
        [Route("api/cart/applyDiscount")]
        public IHttpActionResult ApplyDiscount([FromBody]CartItemRequestData cartItemRequestData)
        {
            var cart = CartHolder.GetCart(USER_KEY);

            var coupon = _discountCouponRepository.GetCouponByKey(cartItemRequestData.CouponKey);
            if (coupon == null)
            {
                return BadRequest("Coupon does not exists");
            }

            cart.ApplyDiscount(coupon);
            return Ok(cart);
        }

        [HttpGet]
        [Route("api/cart/finishOrder")]
        public IHttpActionResult FinishOrder()
        {
            //TODO: could be improved to call Orders Service, then call the Promotion Service and then create and save a order
            CartHolder.DeleteCart(USER_KEY);

            return Ok();
        }


        private bool TryDomainOperation(Action act, out IHttpActionResult httpActionResult)
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

        private IHttpActionResult GetHttpResultFromDomainException(DeveloperShopException ex)
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
