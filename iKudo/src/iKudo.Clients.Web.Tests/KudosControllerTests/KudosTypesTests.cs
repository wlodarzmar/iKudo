using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Enums;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
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
        [Fact]
        public void KudosTypes_ReturnsOk()
        {
            Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();
            Mock<IManageKudos> kudoManagerMock = new Mock<IManageKudos>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            OkObjectResult response = controller.GetKudoTypes() as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void KudosTypes_ReturnsData()
        {
            Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();
            Mock<IManageKudos> kudoManagerMock = new Mock<IManageKudos>();
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
            Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();
            Mock<IManageKudos> kudoManagerMock = new Mock<IManageKudos>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            controller.GetKudoTypes();

            kudoManagerMock.Verify(x => x.GetTypes(), Times.Once);
        }

        [Fact]
        public void KudosTypes_ReturnsInternalServerErrorWhenUnknownExceptionThrown()
        {
            Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();
            Mock<IManageKudos> kudoManagerMock = new Mock<IManageKudos>();
            kudoManagerMock.Setup(x => x.GetTypes()).Throws<Exception>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);

            ObjectResult response = controller.GetKudoTypes() as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }
    }
}
