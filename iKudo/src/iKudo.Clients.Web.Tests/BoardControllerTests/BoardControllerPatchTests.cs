using FluentAssertions;
using iKudo.Domain.Model;
using iKudo.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Xunit;

namespace iKudo.Clients.Web.Tests.BoardControllerTests
{
    public class BoardControllerPatchTests : BoardControllerTestsBase
    {
        [Fact]
        public void Patch_BoardDoesntExist_ReturnsNotFound()
        {
            BoardManagerMock.Setup(x => x.Get(It.IsAny<int>())).Returns((Board)null);

            var patch = new JsonPatchDocument<BoardPatch>();
            patch.Operations.Add(new Microsoft.AspNetCore.JsonPatch.Operations.Operation<BoardPatch>());
            var notFoundResult = Controller.Patch(1, patch) as NotFoundResult;

            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public void Patch_WithoutOperation_ReturnsBadRequest()
        {
            var patch = new JsonPatchDocument<BoardPatch>();
            var badRequestResult = Controller.Patch(1, patch) as BadRequestObjectResult;

            badRequestResult.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            badRequestResult.Value.Should().NotBeNull();
        }
    }
}
