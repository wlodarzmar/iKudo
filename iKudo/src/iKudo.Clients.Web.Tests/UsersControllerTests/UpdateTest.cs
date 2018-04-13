﻿using FluentAssertions;
using iKudo.Clients.Web.Filters;
using iKudo.Controllers.Api;
using iKudo.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq;
using Xunit;

namespace iKudo.Clients.Web.Tests.UsersControllerTests
{
    public class UpdateTest : UsersControllerTestsBase
    {
        [Fact]
        public void UsersController_Put_ReturnsOkObjectResult()
        {
            UserDTO user = GetUserDto();

            IActionResult result = Controller.Update(user) as OkObjectResult;

            result.Should().NotBeNull();
        }

        [Fact]
        public void UsersController_Put_CallsManagerOnce()
        {
            UserDTO user = GetUserDto();

            Controller.Update(user);

            UserManagerMock.Verify(x => x.AddOrUpdate(It.IsAny<User>()));
        }

        [Fact]
        public void UsersContorller_Put_HasValidationFilterAttribute()
        {
            var attribute = Controller.GetType()
                                       .GetMethod(nameof(UsersController.Update))
                                       .CustomAttributes
                                       .SingleOrDefault(x => x.AttributeType == typeof(ValidationFilter));

            attribute.Should().NotBeNull();
        }
    }
}
