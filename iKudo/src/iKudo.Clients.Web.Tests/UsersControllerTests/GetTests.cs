using FluentAssertions;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.UsersControllerTests
{
    public class GetTests : UsersControllerTestsBase
    {
        [Fact]
        public void GetUsers_ReturnsOk()
        {
            OkObjectResult response = Controller.GetUsers(1, null) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void GetUsers_ReturnsUsers()
        {
            DtoFactoryMock.Setup(x => x.Create<UserDto, User>(It.IsAny<IEnumerable<User>>()))
                          .Returns(new List<UserDto> { new UserDto { Id = "id", FirstName = "name" } });

            OkObjectResult response = Controller.GetUsers(1, null) as OkObjectResult;

            response.Should().NotBeNull();
            response.Value.As<IEnumerable<UserDto>>().Count().Should().Be(1);
        }
    }
}
