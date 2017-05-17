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
    public class BoardManagerGetTests : BoardTestsBase
    {
        List<Board> data = new List<Board> {
            new Board { Id = 1, Name = "board name"  , CreatorId="creator", CreationDate = DateTime.Now },
            new Board { Id = 2, Name = "board name 2", CreatorId="creator", CreationDate = DateTime.Now }
        };
        
        [Fact]
        public void BoardManager_GetBoard_Returns_Null_If_Not_Found_Board()
        {
            FillContext(data);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            int boardId = 3;
            Board board = manager.Get(boardId);

            Assert.Null(board);
        }

        [Fact]
        public void BoardManager_GetBoard_Returns_Board()
        {
            FillContext(data);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            int boardId = 1;
            Board board = manager.Get(boardId);

            Assert.NotNull(board);
        }

        [Fact]
        public void BoardManager_GetAll_ReturnsAllBoards()
        {
            FillContext(data);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            ICollection<Board> boards = manager.GetAll();

            Assert.NotNull(boards);
            Assert.Equal(data.Count(), boards.Count);
        }
    }
}
