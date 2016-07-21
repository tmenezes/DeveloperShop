using System.Collections.Generic;
using System.Threading.Tasks;
using DeveloperShop.Domain;

namespace DeveloperShop.Services.GitHub
{
    public interface IGitHubUserService
    {
        Task<Developer> GetUser(string userName);
        Task<IEnumerable<Developer>> GetOrganizationUsers(string organizationName);
    }
}