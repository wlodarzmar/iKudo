using iKudo.Controllers.Api;
using iKudo.Domain.Interfaces;
using iKudo.Dtos;
using iKudo.Parsers;
using Microsoft.Extensions.Logging;
using Moq;

namespace iKudo.Clients.Web.Tests.UsersControllerTests
{
    public class UsersControllerTestsBase
    {
        public UsersControllerTestsBase()
        {
            UserManagerMock = new Mock<IManageUsers>();
            DtoFactoryMock = new Mock<IDtoFactory>();
            ParserMock = new Mock<IUserSearchCriteriaParser>();
            LoggerMock = new Mock<ILogger<UsersController>>();

            Controller = new UsersController(UserManagerMock.Object, DtoFactoryMock.Object, ParserMock.Object, LoggerMock.Object);
        }

        public UsersController Controller { get; set; }

        protected Mock<IManageUsers> UserManagerMock { get; set; }

        protected new Mock<IDtoFactory> DtoFactoryMock { get; set; }

        protected new Mock<IUserSearchCriteriaParser> ParserMock { get; set; }

        protected new Mock<ILogger<UsersController>> LoggerMock { get; set; }

        protected UserDto GetUserDto()
        {
            return new UserDto { Id = "id", FirstName = "name" };
        }
    }
}
