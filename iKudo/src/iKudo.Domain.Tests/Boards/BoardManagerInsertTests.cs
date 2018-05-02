using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class BoardManagerInsertTests : BoardManagerBaseTest
    {
        [Fact]
        public void BoardManager_Throws_ArgumentNullException_If_Board_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Manager.Add(null));
        }

        [Fact]
        public void BoardManager_AddBoard_AddsBoard()
        {
            Board board = new Board() { Name = "board name", CreationDate = DateTime.Now, CreatorId = "asd" };
            Manager.Add(board);

            Assert.Equal(1, DbContext.Boards.Count());
        }

        [Fact]
        public void BoardManager_AddBoard_Returns_AddedBoard()
        {
            DateTime now = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(now);

            Board board = new Board() { Name = "board name", CreatorId = "asd" };
            Board addedBoard = Manager.Add(board);

            addedBoard.Should().NotBeNull();
            board.Name.ShouldBeEquivalentTo(addedBoard.Name);
            board.CreationDate.ShouldBeEquivalentTo(now);
        }

        [Fact]
        public void BoardManager_AddBoard_ThrowsExceptionIfBoardNameExists()
        {
            var data = new List<Board> { new Board { Name = "board name", CreatorId = "asd", CreationDate = DateTime.Now } };
            DbContext.Fill(data);

            Board board = new Board() { Name = "board name", CreationDate = DateTime.Now, CreatorId = "adas" };

            Assert.Throws<AlreadyExistException>(() =>
            {
                Manager.Add(board);
            });
        }

        [Fact]
        public void BoardManager_AddBoard_AddsCreatorToUserBoard()
        {
            Board board = new Board() { Name = "board name", CreationDate = DateTime.Now, CreatorId = "adas" };

            Manager.Add(board);

            DbContext.UserBoards.Any(x => x.BoardId == board.Id && x.UserId == board.CreatorId).Should().BeTrue();
        }

        [Fact]
        public void BoardManager_AddBoard_NewBoardIsPrivateByDefault()
        {
            Board board = new Board { Name = "board", CreatorId = "creator" };

            board = Manager.Add(board);

            board.IsPrivate.Should().BeTrue();
        }

        [Fact]
        public void BoardManager_Update_ThrowsInvalidOperationException_When_PrivateBoard_And_AcceptanceType_ExternalUsersOnly()
        {
            var board = new Board
            {
                Id = 1,
                IsPrivate = true,
                AcceptanceType = AcceptanceType.FromExternalUsersOnly,
                CreatorId = "creatorId",
                Name = "name",
                CreationDate = DateTime.Now
            };

            Assert.Throws<ValidationException>(() => Manager.Add(board));
        }
    }
}
