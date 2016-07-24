using System.Collections.Concurrent;
using DeveloperShop.Domain;

namespace DeveloperShop.Web.Models
{
    public static class CartHolder
    {
        private static readonly ConcurrentDictionary<string, Cart> _carts = new ConcurrentDictionary<string, Cart>();

        public static Cart GetCart(string id)
        {
            var isNewCart = string.IsNullOrEmpty(id);
            if (isNewCart)
                return AddNewCart();

            // get existent cart
            Cart cart;
            if (_carts.TryGetValue(id, out cart))
                return cart;

            // cart doesnt exists, create a new cart based on user id
            cart = new Cart() { Id = id };
            _carts.TryAdd(id, cart);
            return cart;
        }

        public static void DeleteCart(string id)
        {
            Cart oldCart;
            _carts.TryRemove(id, out oldCart);
        }


        private static Cart AddNewCart()
        {
            var cart = new Cart();
            _carts.TryAdd(cart.Id, cart);
            return cart;
        }
    }
}
