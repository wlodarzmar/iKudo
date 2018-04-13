using FluentAssertions;
using iKudo.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Users
{
    public class UserManagerAddTests : UserManagerTestsBase
    {
        [Fact]
        public void UserManager_AddsUserSuccessfully()
        {
            User user = CreateUser();

            UserManager.AddOrUpdate(user);

            User addedUser = DbContext.Users.SingleOrDefault(x => x.Id == user.Id);
            addedUser.Should().NotBeNull();
        }

        [Fact]
        public void UserManager_UpdatesUserIfExists()
        {
            User user = CreateUser();

            DbContext.Fill(new List<User> { user });

            user.FirstName = "first name edited";
            UserManager.AddOrUpdate(user);

            User addedUser = DbContext.Users.SingleOrDefault(x => x.Id == user.Id);
            addedUser.FirstName.Should().Be(user.FirstName);
        }
    }
}
