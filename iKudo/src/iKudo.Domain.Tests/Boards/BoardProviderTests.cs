using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class BoardProviderTests : BaseTest
    {
        private static List<Board> data = new List<Board> {
                new Board { Id = 1, Name = "board name"  , CreatorId="creator", CreationDate = DateTime.Now, IsPrivate = true },
                new Board { Id = 2, Name = "board name 2", CreatorId="creator", CreationDate = DateTime.Now, IsPrivate = true },

                new Board { Id = 3, Name = "board name 3", CreatorId="creator2", CreationDate = DateTime.Now,
                    UserBoards = new List<UserBoard> { new UserBoard("member", 3) }, IsPrivate = true },

                new Board { Id = 4, Name = "board name 4", CreatorId="creator3", CreationDate = DateTime.Now,
                    UserBoards = new List<UserBoard> { new UserBoard("member", 4), new UserBoard("creator3", 4) },
                    IsPrivate = false },

                new Board { Id = 5, Name = "board name 5", CreatorId="creator", CreationDate = DateTime.Now,
                    UserBoards = new List<UserBoard> { new UserBoard("member1", 5), new UserBoard("creator", 5) },
                    IsPrivate = false},
        };

        public BoardProviderTests()
        {
            BoardProvider = new BoardsProvider(DbContext);
            DbContext.Fill(data);
        }

        public IProvideBoards BoardProvider { get; set; }

        [Fact]
        public void BoardManager_GetBoard_Returns_Null_If_Not_Found_Board()
        {
            int boardId = int.MaxValue;
            Board board = BoardProvider.Get(boardId);

            Assert.Null(board);
        }

        [Fact]
        public void BoardManager_GetBoard_Returns_Board()
        {
            int boardId = 1;
            Board board = BoardProvider.Get(boardId);

            Assert.NotNull(board);
        }

        [Fact]
        public void BoardManager_GetAll_ReturnsAllBoardsExceptPrivateAndForeign()
        {
            ICollection<Board> boards = BoardProvider.GetAll("creator2", It.IsAny<BoardSearchCriteria>());

            Assert.NotNull(boards);
            // board 3,4 and 5
            Assert.Equal(3, boards.Count);
        }

        [Fact]
        public void BoardManager_GetAllWithCreatorId_ReturnsBoardsCreatedByGivenUser()
        {
            BoardSearchCriteria criteria = new BoardSearchCriteria { CreatorId = "creator2" };

            ICollection<Board> boards = BoardProvider.GetAll("creator2", criteria);

            boards.Count.Should().Be(1);
        }

        [Fact]
        public void BoardManager_GetAllWithMember_ReturnsBoardsWithGivenMember()
        {
            BoardSearchCriteria criteria = new BoardSearchCriteria { Member = "member" };

            ICollection<Board> boards = BoardProvider.GetAll("creator2", criteria);

            boards.Count.Should().Be(2);
        }

        [Fact]
        public void BoardManager_GetAllWithMemberAndCreator_ReturnsBoardsWithGivenMember()
        {
            BoardSearchCriteria criteria = new BoardSearchCriteria { Member = "creator3", CreatorId = "creator3" };

            ICollection<Board> boards = BoardProvider.GetAll("creator3", criteria);

            boards.Count.Should().Be(1);
        }
    }
}
