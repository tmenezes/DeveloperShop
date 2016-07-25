using System;
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
    public abstract class When_get_developer_from_GitHub
    {
        Developer dev;
        string username;
        Mock<IDeveloperRepository> mockIDeveloperRepository;
        Mock<IGitHubUserService> mockIGitHubUserService;
        DevelopersController controller;

        protected abstract void Arrange();

        [TestFixture]
        public class Given_existent_developer_in_DevShop_database : When_get_developer_from_GitHub
        {
            protected override void Arrange()
            {
                dev = StubFactory.GetDeveloper();
                username = dev.UserName;

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();
                mockIDeveloperRepository.Setup(mock => mock.GetDeveloper(username))
                                        .Returns(dev);

                mockIGitHubUserService = new Mock<IGitHubUserService>();

                controller = new DevelopersController(mockIDeveloperRepository.Object, mockIGitHubUserService.Object);
            }

            [Test]
            public void Should_return_status_code_Ok()
            {
                Arrange();

                // act
                var httpResult = controller.GetFromGitHub(username).Result;

                // assert
                Assert.IsInstanceOf<OkNegotiatedContentResult<Developer>>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                var httpResult = controller.Get(username);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(username), Times.Once);
            }

            [Test]
            public void Should_not_call_GitHub_service()
            {
                Arrange();

                var httpResult = controller.GetFromGitHub(username);

                mockIGitHubUserService.Verify(mock => mock.GetUser(username), Times.Never);
            }
        }

        [TestFixture]
        public class Given_inexistent_developer_in_DevShop_database : When_get_developer_from_GitHub
        {
            protected override void Arrange()
            {
                dev = StubFactory.GetDeveloper();
                username = dev.UserName;

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();

                mockIGitHubUserService = new Mock<IGitHubUserService>();
                mockIGitHubUserService.Setup(mock => mock.GetUser(username))
                                      .Returns(Task.Factory.StartNew(() => dev));

                controller = new DevelopersController(mockIDeveloperRepository.Object, mockIGitHubUserService.Object);
            }

            [Test]
            public void Should_return_status_code_Ok()
            {
                Arrange();

                // act
                var httpResult = controller.GetFromGitHub(username).Result;

                // assert
                Assert.IsInstanceOf<OkNegotiatedContentResult<Developer>>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                var httpResult = controller.Get(username);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(username), Times.Once);
            }

            [Test]
            public void Should_call_GitHub_service()
            {
                Arrange();

                var httpResult = controller.GetFromGitHub(username);

                mockIGitHubUserService.Verify(mock => mock.GetUser(username), Times.Once);
            }
        }

        [TestFixture]
        public class Given_inexistent_profile_in_GitHub : When_get_developer_from_GitHub
        {
            protected override void Arrange()
            {
                dev = null;
                username = "inexistent";

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();

                mockIGitHubUserService = new Mock<IGitHubUserService>();
                mockIGitHubUserService.Setup(mock => mock.GetUser(username))
                                      .Returns(Task.Factory.StartNew(() => dev));

                controller = new DevelopersController(mockIDeveloperRepository.Object, mockIGitHubUserService.Object);
            }

            [Test]
            public void Should_return_status_code_not_found()
            {
                Arrange();

                // act
                var httpResult = controller.GetFromGitHub(username).Result;

                // assert
                Assert.IsInstanceOf<NotFoundResult>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                var httpResult = controller.Get(username);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(username), Times.Once);
            }

            [Test]
            public void Should_call_GitHub_service()
            {
                Arrange();

                var httpResult = controller.GetFromGitHub(username);

                mockIGitHubUserService.Verify(mock => mock.GetUser(username), Times.Once);
            }
        }

        [TestFixture]
        public class Given_exception_in_GitHub_service : When_get_developer_from_GitHub
        {
            protected override void Arrange()
            {
                dev = null;
                username = "inexistent";

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();

                mockIGitHubUserService = new Mock<IGitHubUserService>();
                mockIGitHubUserService.Setup(mock => mock.GetUser(username))
                                      .Throws<ApplicationException>();

                controller = new DevelopersController(mockIDeveloperRepository.Object, mockIGitHubUserService.Object);
            }

            [Test]
            public void Should_return_status_code_internal_server_error()
            {
                Arrange();

                // act
                var httpResult = controller.GetFromGitHub(username).Result;

                // assert
                Assert.IsInstanceOf<ExceptionResult>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                var httpResult = controller.Get(username);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(username), Times.Once);
            }

            [Test]
            public void Should_call_GitHub_service()
            {
                Arrange();

                var httpResult = controller.GetFromGitHub(username);

                mockIGitHubUserService.Verify(mock => mock.GetUser(username), Times.Once);
            }
        }
    }
}