using System.Web.Http.Results;
using DeveloperShop.Domain;
using DeveloperShop.Domain.Repository;
using DeveloperShop.Web.Controllers;
using Moq;
using NUnit.Framework;

namespace DeveloperShop.Tests.Api.DeveloperControllerTests
{
    [TestFixture]
    public abstract class When_get_developer_by_id
    {
        Developer dev;
        int id;
        Mock<IDeveloperRepository> mockIDeveloperRepository;
        DevelopersController controller;

        protected abstract void Arrange();

        [TestFixture]
        public class Given_inexistent_developer : When_get_developer_by_id
        {
            protected override void Arrange()
            {
                id = 10;
                mockIDeveloperRepository = new Mock<IDeveloperRepository>();
                controller = new DevelopersController(mockIDeveloperRepository.Object, null);
            }

            [Test]
            public void Should_return_status_code_not_found()
            {
                Arrange();

                // act
                var httpResult = controller.Get(id);

                // assert
                Assert.IsInstanceOf<NotFoundResult>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.Get(id);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(id), Times.Once);
            }
        }

        [TestFixture]
        public class Given_existent_developer : When_get_developer_by_id
        {
            protected override void Arrange()
            {
                dev = StubFactory.GetDeveloper();
                id = dev.Id;

                mockIDeveloperRepository = new Mock<IDeveloperRepository>();
                mockIDeveloperRepository.Setup(mock => mock.GetDeveloper(id))
                                        .Returns(dev);

                controller = new DevelopersController(mockIDeveloperRepository.Object, null);
            }

            [Test]
            public void Should_return_status_code_Ok()
            {
                Arrange();

                // act
                var httpResult = controller.Get(id);

                // assert
                Assert.IsInstanceOf<OkNegotiatedContentResult<Developer>>(httpResult);
            }

            [Test]
            public void Should_call_repository()
            {
                Arrange();

                controller.Get(id);

                mockIDeveloperRepository.Verify(mock => mock.GetDeveloper(id), Times.Once);
            }
        }
    }
}
