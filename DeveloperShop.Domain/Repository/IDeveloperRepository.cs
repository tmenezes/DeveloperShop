using System.Collections.Generic;

namespace DeveloperShop.Domain.Repository
{
    public interface IDeveloperRepository
    {
        IEnumerable<Developer> GetDevelopers();
        Developer GetDeveloper(int id);
        Developer GetDeveloper(string userName);
        void Add(Developer developer);
    }
}