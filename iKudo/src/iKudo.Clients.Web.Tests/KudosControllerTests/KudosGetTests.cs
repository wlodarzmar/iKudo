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
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.WithCurrentUser("user");

            OkObjectResult response = controller.Get(1) as OkObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void Get_ReturnsKudos()
        {
            IEnumerable<KudoDTO> kudosDto = new List<KudoDTO> { new KudoDTO() };
            dtoFactoryMock.Setup(x => x.Create<KudoDTO, Kudo>(It.IsAny<IEnumerable<Kudo>>())).Returns(kudosDto);
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.WithCurrentUser("user");

            OkObjectResult response = controller.Get(1) as OkObjectResult;

            response.Value.As<IEnumerable<KudoDTO>>().Count().Should().Be(1);
        }

        [Fact]
        public void Get_UnknownExcepthionThrown_ReturnsInternalServerError()
        {
            kudoManagerMock.Setup(x => x.GetKudos(It.IsAny<KudosSearchCriteria>())).Throws<Exception>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);

            ObjectResult response = controller.Get(It.IsAny<int>()) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public void Get_CallsParserWithCurrentUser()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.WithCurrentUser("user");

            controller.Get(It.IsAny<int>());

            kudoSearchCriteriaParserMock.Verify(x => x.Parse(It.Is<string>(p => p == "user"), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Fact]
        public void Get_WithGivenUser_CallsParserWithUser()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.WithCurrentUser("user");

            controller.Get(user: "someUser");

            kudoSearchCriteriaParserMock.Verify(x => x.Parse(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>(), It.Is<string>(p=> p == "someUser")));
        }

        [Fact]
        public void Get_WithGivenSender_CallsParserWithSender()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.WithCurrentUser("user");

            controller.Get(sender: "someSender");

            kudoSearchCriteriaParserMock.Verify(x => x.Parse(It.IsAny<string>(), It.IsAny<int?>(), It.Is<string>(p=>p == "someSender"), It.IsAny<string>(), It.IsAny<string>()));
        }

        [Fact]
        public void Get_WithGivenReceiver_CallsParserWithReceiver()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.WithCurrentUser("user");

            controller.Get(receiver: "someReceiver");

            kudoSearchCriteriaParserMock.Verify(x => x.Parse(It.IsAny<string>(), It.IsAny<int?>(), It.IsAny<string>(), It.Is<string>(p=>p == "someReceiver"), It.IsAny<string>()));
        }
    }
}
