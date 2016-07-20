using System.Linq;
using System.Web.Http;
using DeveloperShop.Domain;
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

            cart.AddItem(developer, cartItemRequestData.AmountOfHours);
            var itemUrl = $"api/cart/{developer.Id}";

            return Created(itemUrl, cart);
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

            cart.RemoveItem(developer);
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
    }
}
