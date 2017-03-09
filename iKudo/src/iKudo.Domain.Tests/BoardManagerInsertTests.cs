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
    public class BoardManagerInsertTests : BoardTestsBase
    {
        [Fact]
        public void BoardManager_Throws_ArgumentNullException_If_Board_Is_Null()
        {
            var kudoDbContextMock = new Mock<KudoDbContext>();
            IBoardManager manager = new BoardManager(kudoDbContextMock.Object);

            Assert.Throws(typeof(ArgumentNullException), () => manager.Add(null));
        }

        [Fact]
        public void BoardManager_Add_Adds_Board()
        {
            IBoardManager manager = new BoardManager(DbContext);

            Board board = new Board() { Name = "board name", CreationDate = DateTime.Now, CreatorId = "asd" };
            manager.Add(board);

            Assert.Equal(1, DbContext.Boards.Count());
        }

        [Fact]
        public void BoardManager_InsertBoard_Returns_AddedBoard()
        {
            DateTime now = DateTime.Now;
            IBoardManager manager = new BoardManager(DbContext);

            Board board = new Board() { Name = "board name", CreationDate = DateTime.Now, CreatorId = "asd" };
            Board addedBoard = manager.Add(board);

            Assert.NotNull(addedBoard);
            Assert.Equal(board.Name, addedBoard.Name);
            Assert.True(board.CreationDate >= now);
        }

        [Fact]
        public void BoardManager_InsertBoard_Throws_Exception_If_Board_Name_Exists()
        {
            var data = new List<Board> { new Board { Name = "board name", CreatorId = "asd", CreationDate = DateTime.Now } };
            FillContext(data);
            IBoardManager manager = new BoardManager(DbContext);

            Board board = new Board() { Name = "board name", CreationDate = DateTime.Now, CreatorId = "adas" };

            Assert.Throws<AlreadyExistException>(() =>
            {
                manager.Add(board);
            });
        }
    }
}
