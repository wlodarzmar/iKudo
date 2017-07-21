using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Users
{
    public class UserManagerGetTests : BaseTest
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
            DbContext.Fill(new List<Board> { board });
            IManageUsers manager = new UserManager(DbContext);

            IEnumerable<User> users = manager.Get(new UserSearchCriteria { BoardId = 1 });

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
            DbContext.Fill(new List<Board> { board });
            IManageUsers manager = new UserManager(DbContext);

            UserSearchCriteria criteria = new UserSearchCriteria { BoardId = 1, Exclude = new string[] { "creator" } };
            IEnumerable<User> users = manager.Get(criteria);

            users.Count().Should().Be(1);
        }
    }
}
