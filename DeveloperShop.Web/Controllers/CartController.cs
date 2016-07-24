using System.Linq;
using System.Web.Http;
using DeveloperShop.Domain;
using DeveloperShop.Domain.Repository;
using DeveloperShop.Web.Models;

namespace DeveloperShop.Web.Controllers
{
    public class CartController : DevShopApiController
    {
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
            return GetCart();
        }

        // GET: api/Cart/:id
        public IHttpActionResult Get(int id)
        {
            var cart = GetCart();
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
            var cart = GetCart();

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
            var cart = GetCart();

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
            var cart = GetCart();

            var coupon = _discountCouponRepository.GetCouponByKey(cartItemRequestData.CouponKey);
            if (coupon == null)
            {
                return BadRequest("Coupon does not exists");
            }

            cart.ApplyDiscount(coupon);
            return Ok(cart);
        }

        [HttpPost]
        [Route("api/cart/finishOrder")]
        public IHttpActionResult FinishOrder()
        {
            //TODO: could be improved to call Orders Service, then call the Promotion Service and then create and save a order
            CartHolder.DeleteCart(UserCartId);

            return Ok();
        }
    }
}
