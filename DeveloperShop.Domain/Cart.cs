using System;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperShop.Domain
{
    public class Cart
    {
        private readonly IList<CartItem> _items;

        public string Id { get; set; }
        public IEnumerable<CartItem> Items
        {
            get { return _items; }
        }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal CartPrice { get; set; }

        public Cart()
        {
            Id = Guid.NewGuid().ToString();
            _items = new List<CartItem>();
        }


        public void AddItem(Developer developer, int amountOfHours)
        {
            var alreadyAdded = _items.Any(d => d.Developer.UserName == developer.UserName);
            if (alreadyAdded)
                return;

            _items.Add(new CartItem(developer, amountOfHours));
            UpdateCartPrices();
        }

        public void RemoveItem(Developer developer)
        {
            var developerAdded = _items.FirstOrDefault(d => d.Developer.UserName == developer.UserName);
            if (developerAdded == null)
                return;

            _items.Remove(developerAdded);
            UpdateCartPrices();
        }


        private void UpdateCartPrices()
        {
            TotalPrice = Items.Sum(i => i.TotalPrice);
            CartPrice = TotalPrice - Discount;
        }
    }
}