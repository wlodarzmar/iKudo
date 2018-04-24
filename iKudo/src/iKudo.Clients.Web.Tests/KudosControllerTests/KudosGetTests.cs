using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.KudosControllerTests
{
    public class KudosGetTests : KudoControllerTestsBase
    {
        [Fact]
        public void Get_ReturnsOk()
        {
            OkObjectResult response = Controller.Get(new KudosSearchCriteria { BoardId = 1 }) as OkObjectResult;

            response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void Get_ReturnsKudos()
        {
            IEnumerable<KudoDto> kudosDto = new List<KudoDto> { new KudoDto() };
            DtoFactoryMock.Setup(x => x.Create<KudoDto, Kudo>(It.IsAny<IEnumerable<Kudo>>())).Returns(kudosDto);

            OkObjectResult response = Controller.Get(new KudosSearchCriteria { BoardId = 1 }) as OkObjectResult;

            response.Value.As<IEnumerable<KudoDto>>().Count().Should().Be(1);
        }
    }
}
