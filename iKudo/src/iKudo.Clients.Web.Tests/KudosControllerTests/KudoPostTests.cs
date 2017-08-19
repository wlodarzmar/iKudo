using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using iKudo.Parsers;
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

        private Mock<IManageKudos> kudoManagerMock;
        private Mock<IUrlHelper> urlHelperMock;
        private Mock<IDtoFactory> dtoFactoryMock;
        private Mock<IKudoSearchCriteriaParser> kudoSearchCriteriaParserMock;

        public KudoPostTests()
        {
            kudoManagerMock = new Mock<IManageKudos>();
            dtoFactoryMock = new Mock<IDtoFactory>();
            urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(LOCATIONURL);
            kudoSearchCriteriaParserMock = new Mock<IKudoSearchCriteriaParser>();
        }

        [Fact]
        public void Add_NotFoundExceptionThrown_ReturnsNotFound()
        {
            kudoManagerMock.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<Kudo>())).Throws<NotFoundException>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.WithCurrentUser("sender");
            KudoDTO newKudoDto = new KudoDTO { };

            NotFoundObjectResult response = controller.Add(newKudoDto) as NotFoundObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Add_UnauthorizedAccessExceptionThrown_ReturnsForbidden()
        {
            kudoManagerMock.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<Kudo>())).Throws<UnauthorizedAccessException>();
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.WithCurrentUser("sender");
            KudoDTO newKudoDto = new KudoDTO { };

            ObjectResult response = controller.Add(newKudoDto) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Add_ValidRequest_ReturnsOk()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.Url = urlHelperMock.Object;
            controller.WithCurrentUser("sender");
            KudoDTO newKudoDto = new KudoDTO { BoardId = 1, Description = "a", ReceiverId = "r", SenderId = "s", Type = new KudoTypeDTO { Id = (int)KudoType.Congratulations } };

            CreatedResult response = controller.Add(newKudoDto) as CreatedResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public void Add_ValidRequest_ReturnsLocationofAddedKudo()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.Url = urlHelperMock.Object;
            controller.WithCurrentUser("sender");
            KudoDTO newKudoDto = new KudoDTO { BoardId = 1, Description = "a", ReceiverId = "r", SenderId = "s", Type = new KudoTypeDTO { Id = (int)KudoType.Congratulations } };

            CreatedResult response = controller.Add(newKudoDto) as CreatedResult;

            response.Location.Should().Be(LOCATIONURL);
        }

        [Fact]
        public void Add_InvalidRequest_ReturnsBadRequestWIthErrors()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.ModelState.AddModelError("key", "error");
            
            KudoDTO newKudoDto = new KudoDTO { };
            BadRequestObjectResult response = controller.Add(newKudoDto) as BadRequestObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            response.Value.Should().NotBeNull();
        }

        [Fact]
        public void Add_UserTriesToAddKudoOnBehalfOfOtherUser_ReturnsForbidden()
        {
            KudosController controller = new KudosController(dtoFactoryMock.Object, kudoManagerMock.Object, kudoSearchCriteriaParserMock.Object);
            controller.Url = urlHelperMock.Object;
            controller.WithCurrentUser("current");
            KudoDTO newKudoDto = new KudoDTO { BoardId = 1, Description = "a", ReceiverId = "r", SenderId = "s", Type = new KudoTypeDTO { Id = (int)KudoType.Congratulations } };
            ObjectResult response = controller.Add(newKudoDto) as ObjectResult;

            kudoManagerMock.Verify(x => x.Add(It.Is<string>(p => p == "current"), It.IsAny<Kudo>()), Times.Once);
        }
    }
}
