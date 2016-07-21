using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeveloperShop.Domain;
using Octokit;

namespace DeveloperShop.Services.GitHub
{
    public class GitHubUserService : IGitHubUserService
    {
        public async Task<Developer> GetUser(string userName)
        {
            try
            {
                var github = GetGitHubClient();
                var user = await github.User.Get(userName);

                return ParseToDeveloper(user);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Developer>> GetOrganizationUsers(string organizationName)
        {
            try
            {
                var github = GetGitHubClient();
                var users = await github.Organization.Member.GetAll(organizationName);

                var developers = users.Select(u => ParseToDeveloper(u)).ToList();

                return developers;
            }
            catch (Exception)
            {
                return new List<Developer>();
            }
        }


        private IGitHubClient GetGitHubClient()
        {
            return new GitHubClient(new ProductHeaderValue("MyAmazingApp"));
        }

        private Developer ParseToDeveloper(User user)
        {
            if (user == null)
                return null;

            var developer = new Developer(user.Id, user.Login)
            {
                FullName = user.Name,
                ImageUrl = user.AvatarUrl,
                Repositories = user.OwnedPrivateRepos + user.PublicRepos,
                Followers = user.Followers,
                Stars = user.Collaborators,
                Following = user.Following,
                Commits = user.Collaborators,
                StartProgrammingDate = user.CreatedAt.DateTime,
            };
            developer.CalculatePrice();

            return developer;
        }
    }
}
