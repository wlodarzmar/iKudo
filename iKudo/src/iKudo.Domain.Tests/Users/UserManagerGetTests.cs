using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Users
{
    public class UserManagerGetTests : UserManagerTestsBase
    {
        [Fact]
        public void UserManager_Get_ReturnsMembersOfBoard()
        {
            Board board = new Board
            {
                Id = 1,
                CreatorId = "creator",
                UserBoards = new List<UserBoard> {
                new UserBoard { BoardId = 1, UserId = "creator" } }
            };
            DbContext.Fill(new List<User> { CreateUser("creator", "fname") });
            DbContext.Fill(new List<Board> { board });

            IEnumerable<User> users = UserManager.Get(new UserSearchCriteria { BoardId = 1 });

            users.Count().Should().Be(1);
        }

        [Fact]
        public void UserManager_Get_DoesntReturnExcludedUsers()
        {
            Board board = new Board
            {
                Id = 1,
                CreatorId = "creator",
                UserBoards = new List<UserBoard> {
                new UserBoard { BoardId = 1, UserId = "creator" },
                new UserBoard { BoardId = 1, UserId = "otherUser" },
                }
            };
            User creator = CreateUser("creator", "fname");
            User otherUser = CreateUser("otherUser", "fname");
            DbContext.Fill(new List<User> { creator, otherUser });
            DbContext.Fill(new List<Board> { board });

            UserSearchCriteria criteria = new UserSearchCriteria { BoardId = 1, Exclude = new string[] { "creator" } };
            IEnumerable<User> users = UserManager.Get(criteria);

            users.Count().Should().Be(1);
        }
    }
}
