using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeveloperShop.Domain;
using Octokit;

namespace DeveloperShop.Services.GitHub
{
    public class GitHubUserService : IGitHubUserService
    {
        // "cache" to avoid unecessary requests to users from an organization
        readonly ConcurrentDictionary<string, IEnumerable<Developer>> _loadedOrganizations = new ConcurrentDictionary<string, IEnumerable<Developer>>();

        // publics
        public async Task<Developer> GetUser(string userName)
        {
            try
            {
                var github = GetGitHubClient();
                var user = await github.User.Get(userName);

                return ParseToDeveloper(user);
            }
            catch (RateLimitExceededException rleex)
            {
                throw new ApplicationException("Github API requests exceeded.", rleex);
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
                if (IsOrganizationLoaded(organizationName))
                    return _loadedOrganizations[organizationName];

                var github = GetGitHubClient();
                var users = await github.Organization.Member.GetAll(organizationName);

                var developers = users.Take(10) // just 10 items because github has a limit conection for "Unauthenticated" connections (60 reqs/hour)
                                      .AsParallel()
                                      .Select(u => ParseToDeveloper(github.User.Get(u.Login).Result))
                                      .ToList();

                _loadedOrganizations.AddOrUpdate(organizationName, developers, (s, devs) => developers);
                return developers;
            }
            catch (Exception)
            {
                return new List<Developer>();
            }
        }


        // privates
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

        private bool IsOrganizationLoaded(string organizationName)
        {
            return _loadedOrganizations.ContainsKey(organizationName);
        }
    }
}
