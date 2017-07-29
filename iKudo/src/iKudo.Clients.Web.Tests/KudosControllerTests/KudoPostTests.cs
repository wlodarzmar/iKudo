using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.KudosControllerTests
{
    public class KudoPostTests
    {
        private const string LOCATIONURL = "location";
        Mock<IUrlHelper> urlHelperMock;

        public KudoPostTests()
        {
            urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(LOCATIONURL);
        }

        [Fact]
        public void Add_NotFoundExceptionThrown_ReturnsNotFound()
        {
            Mock<IManageKudos> kudoManagerMock = new Mock<IManageKudos>();
            kudoManagerMock.Setup(x => x.Insert(It.IsAny<Kudo>())).Throws<NotFoundException>();
            Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);

            KudoDTO newKudoDto = new KudoDTO { };
            NotFoundObjectResult response = controller.Add(newKudoDto) as NotFoundObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Add_UnauthorizedAccessExceptionThrown_ReturnsForbidden()
        {
            Mock<IManageKudos> kudoManagerMock = new Mock<IManageKudos>();
            kudoManagerMock.Setup(x => x.Insert(It.IsAny<Kudo>())).Throws<UnauthorizedAccessException>();
            Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);

            KudoDTO newKudoDto = new KudoDTO { };
            ObjectResult response = controller.Add(newKudoDto) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Add_ValidRequest_ReturnsOk()
        {
            Mock<IManageKudos> kudoManagerMock = new Mock<IManageKudos>();
            Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            controller.Url = urlHelperMock.Object;
            KudoDTO newKudoDto = new KudoDTO { BoardId = 1, Description = "a", ReceiverId = "r", SenderId = "s", Type = new KudoTypeDTO { Id = (int)KudoType.Congratulations } };
            CreatedResult response = controller.Add(newKudoDto) as CreatedResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public void Add_ValidRequest_ReturnsLocationofAddedKudo()
        {
            Mock<IManageKudos> kudoManagerMock = new Mock<IManageKudos>();
            Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            controller.Url = urlHelperMock.Object;
            KudoDTO newKudoDto = new KudoDTO { BoardId = 1, Description = "a", ReceiverId = "r", SenderId = "s", Type = new KudoTypeDTO { Id = (int)KudoType.Congratulations } };
            CreatedResult response = controller.Add(newKudoDto) as CreatedResult;

            response.Location.Should().Be(LOCATIONURL);
        }

        [Fact]
        public void Add_InvalidRequest_ReturnsBadRequestWIthErrors()
        {
            Mock<IManageKudos> kudoManagerMock = new Mock<IManageKudos>();
            Mock<IDtoFactory> dtoFactoryMock = new Mock<IDtoFactory>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object);
            controller.ModelState.AddModelError("key", "error");
            
            KudoDTO newKudoDto = new KudoDTO { };
            BadRequestObjectResult response = controller.Add(newKudoDto) as BadRequestObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            response.Value.Should().NotBeNull();
        }
    }
}
