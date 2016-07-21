using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using DeveloperShop.Domain;
using DeveloperShop.Domain.Repository;
using DeveloperShop.Services.GitHub;

namespace DeveloperShop.Web.Controllers
{
    public class DevelopersController : ApiController
    {
        // fields
        private readonly IDeveloperRepository _developerRepository;
        private readonly IGitHubUserService _gitHubUserService;

        // constructors
        public DevelopersController()
            : this(new DeveloperRepository(), new GitHubUserService())
        {
        }

        public DevelopersController(IDeveloperRepository developerRepository, IGitHubUserService gitHubUserService)
        {
            _developerRepository = developerRepository;
            _gitHubUserService = gitHubUserService;
        }


        // GET: api/Developer
        public IEnumerable<Developer> Get()
        {
            return _developerRepository.GetDevelopers();
        }

        // GET: api/Developer/5
        public IHttpActionResult Get(int id)
        {
            var developer = _developerRepository.GetDeveloper(id);

            if (developer == null)
            {
                return NotFound();
            }

            return Ok(developer);
        }

        // GET: api/Developer/tmenezes
        public IHttpActionResult Get([FromUri] string name)
        {
            var developer = _developerRepository.GetDeveloper(name);

            if (developer == null)
            {
                return NotFound();
            }

            return Ok(developer);
        }

        [Route("api/developers/fromGithub/{username:alpha}")]
        public async Task<IHttpActionResult> GetFromGitHub(string username)
        {
            var developer = _developerRepository.GetDeveloper(username);

            if (developer != null)
            {
                return Ok(developer);
            }


            developer = await _gitHubUserService.GetUser(username);

            if (developer != null)
            {
                _developerRepository.Add(developer);
                return Ok(developer);
            }

            return NotFound();
        }

        [Route("api/developers/fromGithubOrganization/{organization:alpha}")]
        public async Task<IHttpActionResult> GetFromGitHubOrganization(string organization)
        {
            var users = await _gitHubUserService.GetOrganizationUsers(organization);

            return Ok(users);
        }

        // POST: api/Developer
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Developer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Developer/5
        public void Delete(int id)
        {
        }
    }
}
