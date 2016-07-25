using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Results;
using DeveloperShop.Domain;
using DeveloperShop.Domain.Repository;
using DeveloperShop.Services.GitHub;
using DeveloperShop.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace DeveloperShop.Tests.Api.DeveloperControllerTests
{
    [TestFixture]
    public abstract class When_get_organization_developers_from_GitHub
    {
        string organization;
        IEnumerable<Developer> developers;
        Mock<IDeveloperRepository> mockIDeveloperRepository;
        Mock<IGitHubUserService> mockIGitHubUserService;
        DevelopersController controller;

        protected abstract void Arrange();

        [TestFixture]
        public class Given_existent_organization_in_GitHub : When_get_organization_developers_from_GitHub
        {
            protected override void Arrange()
            {
                developers = new List<Developer>()
                {
                    StubFactory.GetDeveloper(),
                    StubFactory.GetDeveloper(),
                    StubFactory.GetDeveloper(),
                };
                organization = "microsoft";

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();

                mockIGitHubUserService = new Mock<IGitHubUserService>();
                mockIGitHubUserService.Setup(mock => mock.GetOrganizationUsers(organization))
                                      .Returns(Task.Factory.StartNew(() => developers));

                controller = new DevelopersController(mockIDeveloperRepository.Object, mockIGitHubUserService.Object);
            }

            [Test]
            public void Should_return_status_code_Ok()
            {
                Arrange();

                // act
                var httpResult = controller.GetFromGitHubOrganization(organization).Result;

                // assert
                Assert.IsInstanceOf<OkNegotiatedContentResult<IEnumerable<Developer>>>(httpResult);
            }

            [Test]
            public void Should_call_GitHub_service()
            {
                Arrange();

                var httpResult = controller.GetFromGitHubOrganization(organization);

                mockIGitHubUserService.Verify(mock => mock.GetOrganizationUsers(organization), Times.Once);
            }
        }

        [TestFixture]
        public class Given_inexistent_organization_in_GitHub : When_get_organization_developers_from_GitHub
        {
            protected override void Arrange()
            {
                developers = new List<Developer>();
                organization = "inexistent";

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();

                mockIGitHubUserService = new Mock<IGitHubUserService>();
                mockIGitHubUserService.Setup(mock => mock.GetOrganizationUsers(organization))
                                      .Returns(Task.Factory.StartNew(() => developers));

                controller = new DevelopersController(mockIDeveloperRepository.Object, mockIGitHubUserService.Object);
            }

            [Test]
            public void Should_return_status_code_Ok_with_empty_return()
            {
                Arrange();

                // act
                var httpResult = controller.GetFromGitHubOrganization(organization).Result;
                var content = (httpResult as OkNegotiatedContentResult<IEnumerable<Developer>>)?.Content;

                // assert
                Assert.IsInstanceOf<OkNegotiatedContentResult<IEnumerable<Developer>>>(httpResult);
                Assert.NotNull(content);
                Assert.AreEqual(0, content.Count());
            }

            [Test]
            public void Should_call_GitHub_service()
            {
                Arrange();

                var httpResult = controller.GetFromGitHubOrganization(organization);

                mockIGitHubUserService.Verify(mock => mock.GetOrganizationUsers(organization), Times.Once);
            }
        }

        [TestFixture]
        public class Given_exception_in_GitHub_service : When_get_organization_developers_from_GitHub
        {
            protected override void Arrange()
            {
                organization = "inexistent";

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();

                mockIGitHubUserService = new Mock<IGitHubUserService>();
                mockIGitHubUserService.Setup(mock => mock.GetOrganizationUsers(organization))
                                      .Throws<ApplicationException>();

                controller = new DevelopersController(mockIDeveloperRepository.Object, mockIGitHubUserService.Object);
            }

            [Test]
            public void Should_return_status_code_internal_server_error()
            {
                Arrange();

                // act
                var httpResult = controller.GetFromGitHubOrganization(organization).Result;

                // assert
                Assert.IsInstanceOf<ExceptionResult>(httpResult);
            }

            [Test]
            public void Should_call_GitHub_service()
            {
                Arrange();

                var httpResult = controller.GetFromGitHubOrganization(organization);

                mockIGitHubUserService.Verify(mock => mock.GetOrganizationUsers(organization), Times.Once);
            }
        }
    }
}