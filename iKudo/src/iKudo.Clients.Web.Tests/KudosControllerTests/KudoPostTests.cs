﻿using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.KudosControllerTests
{
    public class KudoPostTests : KudoControllerTestsBase
    {
        private const string LOCATIONURL = "location";

        private Mock<IUrlHelper> urlHelperMock;

        public KudoPostTests()
        {
            urlHelperMock = new Mock<IUrlHelper>();
            urlHelperMock.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(LOCATIONURL);
        }

        [Fact]
        public void Add_NotFoundExceptionThrown_ReturnsNotFound()
        {
            KudoManagerMock.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<Kudo>())).Throws<NotFoundException>();
            KudoDTO newKudoDto = new KudoDTO { };

            NotFoundObjectResult response = Controller.Add(newKudoDto) as NotFoundObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Add_UnauthorizedAccessExceptionThrown_ReturnsForbidden()
        {
            KudoManagerMock.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<Kudo>())).Throws<UnauthorizedAccessException>();
            KudoDTO newKudoDto = new KudoDTO { };

            ObjectResult response = Controller.Add(newKudoDto) as ObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Forbidden);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Add_ValidRequest_ReturnsOk()
        {
            Controller.Url = urlHelperMock.Object;
            KudoDTO newKudoDto = new KudoDTO { BoardId = 1, Description = "a", ReceiverId = "r", SenderId = "s", Type = new KudoTypeDTO { Id = (int)KudoType.Congratulations } };

            CreatedResult response = Controller.Add(newKudoDto) as CreatedResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.Created);
        }

        [Fact]
        public void Add_ValidRequest_ReturnsLocationofAddedKudo()
        {
            Controller.Url = urlHelperMock.Object;
            KudoDTO newKudoDto = new KudoDTO { BoardId = 1, Description = "a", ReceiverId = "r", SenderId = "s", Type = new KudoTypeDTO { Id = (int)KudoType.Congratulations } };

            CreatedResult response = Controller.Add(newKudoDto) as CreatedResult;

            response.Location.Should().Be(LOCATIONURL);
        }

        [Fact]
        public void Add_UserTriesToAddKudoOnBehalfOfOtherUser_ReturnsForbidden()
        {
            Controller.Url = urlHelperMock.Object;
            Controller.WithCurrentUser("current");
            KudoDTO newKudoDto = new KudoDTO { BoardId = 1, Description = "a", ReceiverId = "r", SenderId = "s", Type = new KudoTypeDTO { Id = (int)KudoType.Congratulations } };

            ObjectResult response = Controller.Add(newKudoDto) as ObjectResult;

            KudoManagerMock.Verify(x => x.Add(It.Is<string>(p => p == "current"), It.IsAny<Kudo>()), Times.Once);
        }

        [Fact]
        public void Add_InvalidOperationExceptionThrown_ReturnsInternalServerError()
        {
            KudoManagerMock.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<Kudo>())).Throws(new InvalidOperationException("some error"));
            Controller.Url = urlHelperMock.Object;

            ObjectResult response = Controller.Add(It.IsAny<KudoDTO>()) as ObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.Value.As<ErrorResult>().Error.Should().NotBeNullOrWhiteSpace();
        }
    }
}
