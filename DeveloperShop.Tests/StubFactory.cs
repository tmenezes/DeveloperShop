using System;
using DeveloperShop.Domain;

namespace DeveloperShop.Tests
{
    public static class StubFactory
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);


        public static Developer GetDeveloper()
        {
            return GetDeveloper(150);
        }

        public static Developer GetDeveloper(decimal price)
        {
            return new Developer(Random.Next(), "_tmenezes_")
            {
                ImageUrl = "https://avatars.githubusercontent.com/u/91613?v=3",
                Followers = 20,
                Repositories = 5,
                Following = 10,
                Price = price
            };
        }
    }
}