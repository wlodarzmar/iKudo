using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;
using System;
using iKudo.Domain.Criteria;
using iKudo.Parsers;

namespace iKudo.Clients.Web.Tests.KudosControllerTests
{
    public class KudosGetTests
    {
        private Mock<IManageKudos> kudoManagerMock;
        private Mock<IDtoFactory> dtoFactoryMock;
        private Mock<IKudoSearchCriteriaParser> kudoSearchCriteriaParserMock;

        public KudosGetTests()
        {
            kudoManagerMock = new Mock<IManageKudos>();
            dtoFactoryMock = new Mock<IDtoFactory>();
            kudoSearchCriteriaParserMock = new Mock<IKudoSearchCriteriaParser>();
        }

        [Fact]
        public void Get_ReturnsOk()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            controller.WithCurrentUser("user");

            OkObjectResult response = controller.Get(new KudosSearchCriteria { BoardId = 1 }) as OkObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void Get_ReturnsKudos()
        {
            IEnumerable<KudoDTO> kudosDto = new List<KudoDTO> { new KudoDTO() };
            dtoFactoryMock.Setup(x => x.Create<KudoDTO, Kudo>(It.IsAny<IEnumerable<Kudo>>())).Returns(kudosDto);
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            controller.WithCurrentUser("user");

            OkObjectResult response = controller.Get(new KudosSearchCriteria { BoardId = 1 }) as OkObjectResult;

            response.Value.As<IEnumerable<KudoDTO>>().Count().Should().Be(1);
        }

        [Fact]
        public void Get_UnknownExcepthionThrown_ReturnsInternalServerError()
        {
            kudoManagerMock.Setup(x => x.GetKudos(It.IsAny<KudosSearchCriteria>())).Throws<Exception>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);

            ObjectResult response = controller.Get(It.IsAny<KudosSearchCriteria>()) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
