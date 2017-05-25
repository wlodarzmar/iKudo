using FluentAssertions;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class BoardManagerUpdateTests : BoardTestsBase
    {
        [Fact]
        public void BoardManager_Throws_ArgumentNullException_If_Board_Is_Null()
        {
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Assert.Throws(typeof(ArgumentNullException), () => manager.Update(null));
        }

        [Fact]
        public void BoardManager_Update_Assign_New_ModificationDate()
        {
            DateTime date = DateTime.Now;
            TimeProviderMock.Setup(x => x.Now()).Returns(date);
            DateTime oldDate = DateTime.Now.AddDays(-5);
            List<Board> boards = new List<Board> {
                new Board { Id =1, Name = "old name", ModificationDate = oldDate, CreationDate = oldDate },
            };
            DbContext.Fill(boards);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Board board = boards.First();
            board.Name = "new name";
            manager.Update(board);

            board.ModificationDate.ShouldBeEquivalentTo(date);
        }

        [Fact]
        public void BoardManager_Update_Throws_NotFoundException_If_Board_Not_Exist()
        {
            List<Board> boards = new List<Board> {
                new Board { Id = 1, Name = "old name", CreationDate = DateTime.Now },
            };
            DbContext.Fill(boards);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Board board = new Board { Id = 345, Name = "new name" };

            Assert.Throws(typeof(NotFoundException), () => manager.Update(board));
        }

        [Fact]
        public void BoardManager_Update_Throws_BoardAlreadyExist_If_Board_With_Given_Name_Exists()
        {
            List<Board> boards = new List<Board> {
                new Board { Id =1, Name = "old name", CreationDate = DateTime.Now},
                new Board { Id =3, Name = "old name 2", CreationDate = DateTime.Now },
            };
            DbContext.Fill(boards);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Board board = new Board { Id = 3, Name = "old name" };

            Assert.Throws(typeof(AlreadyExistException), () => manager.Update(board));
        }

        [Fact]
        public void BoardManager_Update_Dont_Throw_BoardAlreadyExistException_If_Board_Name_Exist_In_Edited_Board()
        {
            List<Board> boards = new List<Board> {
                new Board { Id = 1, Name = "old name", CreationDate = DateTime.Now},
                new Board { Id = 3, Name = "old name 2", CreationDate = DateTime.Now },
            };
            DbContext.Fill(boards);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Board board = boards.First();
            board.Description = "desc";

            manager.Update(board);
        }

        [Fact]
        public void BoardManager_Throws_UnauthorizedAccess_If_Edit_Foreign_Board()
        {
            List<Board> boards = new List<Board> {
                new Board { Id = 1, CreatorId = "creatorId", Name = "old name", CreationDate = DateTime.Now},
            };
            DbContext.Fill(boards);
            IBoardManager manager = new BoardManager(DbContext, TimeProviderMock.Object);

            Board board = new Board { Id = 1, CreatorId = "otherCreatorId", Description = "desc", Name = "old name 2" };

            Assert.Throws(typeof(UnauthorizedAccessException), () => manager.Update(board));
        }
    }
}
