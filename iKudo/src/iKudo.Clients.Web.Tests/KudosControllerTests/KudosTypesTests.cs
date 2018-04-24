using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Dtos;
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
            DtoFactoryMock.Setup(x => x.Create<KudoTypeDto, KudoType>(It.IsAny<IEnumerable<KudoType>>()))
                .Returns(new List<KudoTypeDto> { new KudoTypeDto { Id = 2, Name = "name" } });
            OkObjectResult response = Controller.GetKudoTypes() as OkObjectResult;

            response.Value.Should().NotBeNull();
            response.Value.As<IEnumerable<KudoTypeDto>>().Count().Should().Be(1);
        }

        [Fact]
        public void KudosTypes_CallsGetTypes()
        {
            Controller.GetKudoTypes();

            KudoManagerMock.Verify(x => x.GetTypes(), Times.Once);
        }
    }
}
