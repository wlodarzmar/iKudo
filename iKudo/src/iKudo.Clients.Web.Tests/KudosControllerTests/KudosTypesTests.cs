using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using iKudo.Parsers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.KudosControllerTests
{
    public class KudosTypesTests
    {
        private Mock<IManageKudos> kudoManagerMock;
        private Mock<IDtoFactory> dtoFactoryMock;
        private Mock<IKudoSearchCriteriaParser> kudoSearchCriteriaParserMock;

        public KudosTypesTests()
        {
            kudoManagerMock = new Mock<IManageKudos>();
            dtoFactoryMock = new Mock<IDtoFactory>();
            kudoSearchCriteriaParserMock = new Mock<IKudoSearchCriteriaParser>();
        }

        [Fact]
        public void KudosTypes_ReturnsOk()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            OkObjectResult response = controller.GetKudoTypes() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void KudosTypes_ReturnsData()
        {
            dtoFactoryMock.Setup(x => x.Create<KudoTypeDTO, KudoType>(It.IsAny<IEnumerable<KudoType>>()))
                .Returns(new List<KudoTypeDTO> { new KudoTypeDTO { Id = 2, Name = "name" } });
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            OkObjectResult response = controller.GetKudoTypes() as OkObjectResult;

            response.Value.Should().NotBeNull();
            response.Value.As<IEnumerable<KudoTypeDTO>>().Count().Should().Be(1);
        }

        [Fact]
        public void KudosTypes_CallsGetTypes()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            controller.GetKudoTypes();

            kudoManagerMock.Verify(x => x.GetTypes(), Times.Once);
        }
    }
}
