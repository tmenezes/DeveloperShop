using System.Collections.Concurrent;
using DeveloperShop.Domain;

namespace DeveloperShop.Web.Models
{
    public static class CartHolder
    {
        private static readonly ConcurrentDictionary<string, Cart> _carts = new ConcurrentDictionary<string, Cart>();

        public static Cart GetCart(string id)
        {
            return _carts.GetOrAdd(id, new Cart());
        }

        public static void DeleteCart(string id)
        {
            Cart oldCart;
            _carts.TryRemove(id, out oldCart);
        }
    }
}
