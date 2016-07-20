using System.Threading.Tasks;
using DeveloperShop.Domain;

namespace DeveloperShop.Services.GitHub
{
    public interface IGitHubUserService
    {
        Task<Developer> GetUser(string userName);
    }
}