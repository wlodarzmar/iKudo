using FluentAssertions;
using iKudo.Domain.Exceptions;
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
    public class BoardManagerInsertTests : BaseTest
    {
        [Fact]
        public void BoardManager_Throws_ArgumentNullException_If_Board_Is_Null()
        {            
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Assert.Throws<ArgumentNullException>(() => manager.Add(null));
        }

        [Fact]
        public void BoardManager_AddBoard_AddsBoard()
        {
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Board board = new Board() { Name = "board name", CreationDate = DateTime.Now, CreatorId = "asd" };
            manager.Add(board);

            Assert.Equal(1, DbContext.Boards.Count());
        }

        [Fact]
        public void BoardManager_AddBoard_Returns_AddedBoard()
        {
            DateTime now = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(now);
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Board board = new Board() { Name = "board name", CreatorId = "asd" };
            Board addedBoard = manager.Add(board);

            addedBoard.Should().NotBeNull();
            board.Name.ShouldBeEquivalentTo(addedBoard.Name);
            board.CreationDate.ShouldBeEquivalentTo(now);
        }

        [Fact]
        public void BoardManager_AddBoard_ThrowsExceptionIfBoardNameExists()
        {
            var data = new List<Board> { new Board { Name = "board name", CreatorId = "asd", CreationDate = DateTime.Now } };
            DbContext.Fill(data);
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Board board = new Board() { Name = "board name", CreationDate = DateTime.Now, CreatorId = "adas" };

            Assert.Throws<AlreadyExistException>(() =>
            {
                manager.Add(board);
            });
        }

        [Fact]
        public void BoardManager_AddBoard_AddsCreatorToUserBoard()
        {
            IManageBoards manager = new BoardManager(DbContext, TimeProviderMock.Object);
            Board board = new Board() { Name = "board name", CreationDate = DateTime.Now, CreatorId = "adas" };

            manager.Add(board);

            DbContext.UserBoards.Any(x => x.BoardId == board.Id && x.UserId == board.CreatorId).Should().BeTrue();
        }
    }
}
