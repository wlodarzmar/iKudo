using FluentAssertions;
using iKudo.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.KudosControllerTests
{
    public class KudosTypesTests : KudoControllerTestsBase
    {
        [Fact]
        public void KudosTypes_ReturnsOk()
        {
            OkObjectResult response = Controller.GetKudoTypes() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void KudosTypes_ReturnsData()
        {
            DtoFactoryMock.Setup(x => x.Create<KudoTypeDTO, KudoType>(It.IsAny<IEnumerable<KudoType>>()))
                .Returns(new List<KudoTypeDTO> { new KudoTypeDTO { Id = 2, Name = "name" } });
            OkObjectResult response = Controller.GetKudoTypes() as OkObjectResult;

            response.Value.Should().NotBeNull();
            response.Value.As<IEnumerable<KudoTypeDTO>>().Count().Should().Be(1);
        }

        [Fact]
        public void KudosTypes_CallsGetTypes()
        {
            Controller.GetKudoTypes();

            KudoManagerMock.Verify(x => x.GetTypes(), Times.Once);
        }
    }
}
