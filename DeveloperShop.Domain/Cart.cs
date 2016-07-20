using System;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperShop.Domain
{
    public class Cart
    {
        private readonly IList<Developer> _items;

        public string Id { get; set; }
        public IEnumerable<Developer> Items
        {
            get { return _items; }
        }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal CartPrice { get; set; }

        public Cart()
        {
            Id = Guid.NewGuid().ToString();
            _items = new List<Developer>();
        }


        public void AddItem(Developer developer)
        {
            var alreadyAdded = _items.Any(d => d.UserName == developer.UserName);
            if (alreadyAdded)
                return;

            _items.Add(developer);
            UpdateCartPrices();
        }

        public void RemoveItem(Developer developer)
        {
            var developerAdded = _items.FirstOrDefault(d => d.UserName == developer.UserName);
            if (developerAdded == null)
                return;

            _items.Remove(developerAdded);
            UpdateCartPrices();
        }


        private void UpdateCartPrices()
        {
            TotalPrice = Items.Sum(d => d.Price);
            CartPrice = TotalPrice - Discount;
        }
    }
}