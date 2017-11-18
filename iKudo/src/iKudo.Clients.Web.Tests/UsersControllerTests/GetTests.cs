using FluentAssertions;
using iKudo.Controllers.Api;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Model;
using iKudo.Dtos;
using iKudo.Parsers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace iKudo.Clients.Web.Tests.UsersControllerTests
{
    public class GetTests
    {
        Mock<IUserSearchCriteriaParser> parserMock;
        Mock<IDtoFactory> dtoFactoryMock;

        public GetTests()
        {
            parserMock = new Mock<IUserSearchCriteriaParser>();
            dtoFactoryMock = new Mock<IDtoFactory>();
        }

        [Fact]
        public void GetUsers_ReturnsOk()
        {
            Mock<IManageUsers> userManagerMock = new Mock<IManageUsers>();
            UsersController controller = new UsersController(userManagerMock.Object, dtoFactoryMock.Object, parserMock.Object);

            OkObjectResult response = controller.GetUsers(1, null) as OkObjectResult;

            response.Should().NotBeNull();
            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void GetUsers_ReturnsUsers()
        {
            Mock<IManageUsers> userManagerMock = new Mock<IManageUsers>();
            dtoFactoryMock.Setup(x => x.Create<UserDTO, User>(It.IsAny<IEnumerable<User>>()))
                          .Returns(new List<UserDTO> { new UserDTO { Id = "id", Name = "name" } });
            UsersController controller = new UsersController(userManagerMock.Object, dtoFactoryMock.Object, parserMock.Object);

            OkObjectResult response = controller.GetUsers(1, null) as OkObjectResult;

            response.Should().NotBeNull();
            response.Value.As<IEnumerable<UserDTO>>().Count().Should().Be(1);
        }
    }
}
