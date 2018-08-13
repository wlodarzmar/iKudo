using FluentAssertions;
using iKudo.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using Xunit;
namespace iKudo.Clients.Web.Tests.KudosControllerTests
{
    public class KudosDeleteTests : KudoControllerTestsBase
    {
        [Fact]
        public void Delete_SuccessfulRemoval_ReturnsOk()
        {
            Controller.WithCurrentUser("user");

            var result = Controller.Delete(1) as OkResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void Delete_KudoManagerDeleteThrowsNotFoundException_ReturnsNotFound()
        {
            Controller.WithCurrentUser("user");
            KudoManagerMock.Setup(x => x.Delete("user", int.MaxValue)).Throws<NotFoundException>();

            var result = Controller.Delete(int.MaxValue) as NotFoundObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public void Delete_KudoManagerDeleteThrowsUnauthorizedAccessException_ReturnsUnauthorized()
        {
            Controller.WithCurrentUser("user");
            KudoManagerMock.Setup(x => x.Delete("user", int.MaxValue)).Throws<UnauthorizedAccessException>();

            var result = Controller.Delete(int.MaxValue) as ObjectResult;

            result.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }
    }
}
