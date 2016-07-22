using System;
using DeveloperShop.Domain;
using NUnit.Framework;

namespace DeveloperShop.Tests.Domain.DeveloperTests
{
    [TestFixture]
    public class When_calculate_your_own_price
    {
        [TestCase(04, 01, 02, 4.92, 3.57)]  // tmenezes
        [TestCase(02, 07, 05, 4.05, 2.65)]  // gabrielmedeiros
        [TestCase(04, 13, 08, 4.71, 5.13)]  // rodrigo-andrade
        [TestCase(10, 10, 20, 0.00, 10.0)]  // (8 + 1 + 1) * 1
        [TestCase(10, 10, 20, 10.0, 11.0)]  // (8 + 1 + 1) * 1.10
        public void Price_should_be_right(int repos, int followers, int following, double years, decimal expectedPrice)
        {
            var days = years * 365;
            var developer = new Developer()
            {
                Repositories = repos,
                Followers = followers,
                Following = following,
                StartProgrammingDate = DateTime.Now.AddDays(-days)
            };

            developer.CalculatePrice();

            Assert.AreEqual(expectedPrice, developer.Price);
        }
    }
}
