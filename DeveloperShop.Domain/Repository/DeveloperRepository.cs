﻿using System.Collections.Generic;
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
                    Followers = 10,
                    Commits = 200,
                    Repositories = 5,
                    Price = 150
                },
                new Developer(2, "fulano")
                {
                    Followers = 50,
                    Commits = 999999999,
                    Repositories = 50,
                    Price = 999
                }
            };
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
