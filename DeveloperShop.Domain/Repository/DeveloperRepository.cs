using System;
using System.Collections.Generic;
using System.Linq;

namespace DeveloperShop.Domain.Repository
{
    public class DeveloperRepository : IDeveloperRepository
    {
        private static readonly IList<Developer> _developers;

        static DeveloperRepository()
        {
            _developers = new List<Developer>()
            {
                new Developer(1, "_tmenezes_")
                {
                    ImageUrl = "https://avatars.githubusercontent.com/u/91613?v=3",
                    Followers = 20,
                    Repositories = 5,
                    Following = 10,
                    StartProgrammingDate = DateTime.Now.AddYears(-5)
                },
                new Developer(2, "fulano")
                {
                    ImageUrl = "https://avatars.githubusercontent.com/u/91613?v=3",
                    Followers = 200,
                    Repositories = 50,
                    Following = 5,
                    StartProgrammingDate = DateTime.Now.AddYears(-10)
                }
            };
            _developers.ToList().ForEach(dev => dev.CalculatePrice());
        }


        public IEnumerable<Developer> GetDevelopers()
        {
            return _developers;
        }

        public Developer GetDeveloper(int id)
        {
            return _developers.SingleOrDefault(d => d.Id == id);
        }

        public Developer GetDeveloper(string userName)
        {
            return _developers.SingleOrDefault(d => d.UserName == userName);
        }

        public void Add(Developer developer)
        {
            var exists = _developers.Any(d => d.UserName == developer.UserName);
            if (exists)
                return;

            _developers.Add(developer);
        }
    }
}
