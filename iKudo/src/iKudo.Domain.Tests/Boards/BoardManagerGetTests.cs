using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class BoardManagerGetTests : BaseTest
    {
        private static List<Board> data = new List<Board> {
                new Board { Id = 1, Name = "board name"  , CreatorId="creator", CreationDate = DateTime.Now },
                new Board { Id = 2, Name = "board name 2", CreatorId="creator", CreationDate = DateTime.Now },

                new Board { Id = 3, Name = "board name 3", CreatorId="creator2", CreationDate = DateTime.Now,
                    UserBoards = new List<UserBoard> { new UserBoard("member", 3) } },

                new Board { Id = 4, Name = "board name 4", CreatorId="creator3", CreationDate = DateTime.Now,
                    UserBoards = new List<UserBoard> { new UserBoard("member", 4), new UserBoard("creator3", 4) } },
        };

        public BoardManagerGetTests()
        {
            DbContext.Fill(data);
        }

        [Fact]
        public void BoardManager_GetBoard_Returns_Null_If_Not_Found_Board()
        {
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);

            int boardId = int.MaxValue;
            Board board = manager.Get(boardId);

            Assert.Null(board);
        }

        [Fact]
        public void BoardManager_GetBoard_Returns_Board()
        {
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);

            int boardId = 1;
            Board board = manager.Get(boardId);

            Assert.NotNull(board);
        }

        [Fact]
        public void BoardManager_GetAll_ReturnsAllBoards()
        {
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);

            ICollection<Board> boards = manager.GetAll(It.IsAny<BoardSearchCriteria>());

            Assert.NotNull(boards);
            Assert.Equal(data.Count(), boards.Count);
        }

        [Fact]
        public void BoardManager_GetAllWithCreatorId_ReturnsBoardsCreatedByGivenUser()
        {
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);
            BoardSearchCriteria criteria = new BoardSearchCriteria { CreatorId = "creator2" };

            ICollection<Board> boards = manager.GetAll(criteria);

            boards.Count.Should().Be(1);
        }

        [Fact]
        public void BoardManager_GetAllWithMember_ReturnsBoardsWithGivenMember()
        {
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);
            BoardSearchCriteria criteria = new BoardSearchCriteria { Member = "member" };

            ICollection<Board> boards = manager.GetAll(criteria);

            boards.Count.Should().Be(2);
        }

        [Fact]
        public void BoardManager_GetAllWithMemberAndCreator_ReturnsBoardsWithGivenMember()
        {
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);
            BoardSearchCriteria criteria = new BoardSearchCriteria { Member = "creator3", CreatorId = "creator3" };

            ICollection<Board> boards = manager.GetAll(criteria);

            boards.Count.Should().Be(1);
        }
    }
}
