using System.Web.Http.Results;
using DeveloperShop.Domain;
using DeveloperShop.Domain.Repository;
using DeveloperShop.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace DeveloperShop.Tests.Api.DeveloperControllerTests
{
    [TestFixture]
    public abstract class When_get_developer_by_name
    {
        Developer dev;
        string name;
        Mock<IDeveloperRepository> mockIDeveloperRepository;
        DevelopersController controller;

        protected abstract void Arrange();

        [TestFixture]
        public class Given_inexistent_developer : When_get_developer_by_name
        {
            protected override void Arrange()
            {
                name = "tmenezes";
                mockIDeveloperRepository = new Mock<IDeveloperRepository>();
                controller = new DevelopersController(mockIDeveloperRepository.Object, null);
            }

            [Test]
            public void Should_return_status_code_not_found()
            {
                Arrange();

                // act
                var httpResult = controller.Get(name);

                // assert
                Assert.IsInstanceOf<NotFoundResult>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.Get(name);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(name), Times.Once);
            }
        }

        [TestFixture]
        public class Given_existent_developer : When_get_developer_by_name
        {
            protected override void Arrange()
            {
                dev = StubFactory.GetDeveloper();
                name = dev.UserName;

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();
                mockIDeveloperRepository.Setup(mock => mock.GetDeveloper(name))
                                        .Returns(dev);

                controller = new DevelopersController(mockIDeveloperRepository.Object, null);
            }

            [Test]
            public void Should_return_status_code_Ok()
            {
                Arrange();

                // act
                var httpResult = controller.Get(name);

                // assert
                Assert.IsInstanceOf<OkNegotiatedContentResult<Developer>>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.Get(name);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(name), Times.Once);
            }
        }
    }
}