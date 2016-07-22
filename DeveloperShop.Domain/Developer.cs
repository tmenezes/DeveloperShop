using System;

namespace DeveloperShop.Domain
{
    public class Developer
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Repositories { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public DateTime StartProgrammingDate { get; set; }


        public Developer()
        {
        }

        public Developer(int id, string userName)
        {
            Id = id;
            UserName = userName;
        }

        public void CalculatePrice()
        {
            var experienceYears = (DateTime.Now - StartProgrammingDate).TotalDays / 365;
            var experienceFactor = (decimal)(experienceYears / 10) + 1;

            var price = (Repositories * 0.8m) + (Followers * 0.1m) + (Following * 0.05m);
            price = price * experienceFactor; // adjuste price according year of experience 

            Price = Math.Round(price, 2);
        }
    }
}
