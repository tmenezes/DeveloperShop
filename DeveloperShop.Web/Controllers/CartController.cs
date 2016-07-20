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

        public CartController()
            : this(new DeveloperRepository())
        {
        }

        public CartController(IDeveloperRepository developerRepository)
        {
            _developerRepository = developerRepository;
        }


        // GET: api/Cart
        public Cart Get()
        {
            return CartHolder.GetCart(USER_KEY);
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
            return Ok(cart);
        }

        // DELETE: api/Cart/
        public IHttpActionResult Delete(CartItemRequestData cartItemRequestData)
        {
            var cart = CartHolder.GetCart(USER_KEY);

            var developer = _developerRepository.GetDeveloper(cartItemRequestData.DeveloperId);
            if (developer == null)
            {
                return BadRequest("Developer was not added on the cart");
            }

            cart.RemoveItem(developer);
            return Ok(cart);
        }
    }
}
