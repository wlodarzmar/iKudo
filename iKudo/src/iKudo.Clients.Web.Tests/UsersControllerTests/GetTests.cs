using FluentAssertions;
using iKudo.Domain.Model;
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
            DtoFactoryMock.Setup(x => x.Create<UserDTO, User>(It.IsAny<IEnumerable<User>>()))
                          .Returns(new List<UserDTO> { new UserDTO { Id = "id", FirstName = "name" } });

            OkObjectResult response = Controller.GetUsers(1, null) as OkObjectResult;

            response.Should().NotBeNull();
            response.Value.As<IEnumerable<UserDTO>>().Count().Should().Be(1);
        }
    }
}
