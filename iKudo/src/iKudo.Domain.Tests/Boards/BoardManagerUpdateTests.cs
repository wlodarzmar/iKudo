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
    public class BoardManagerUpdateTests : BoardManagerBaseTest
    {
        [Fact]
        public void BoardManager_Throws_ArgumentNullException_If_Board_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => Manager.Update(null));
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

            Board board = boards.First();
            board.Name = "new name";
            Manager.Update(board);

            board.ModificationDate.Should().Be(date);
        }

        [Fact]
        public void BoardManager_Update_Throws_NotFoundException_If_Board_Not_Exist()
        {
            List<Board> boards = new List<Board> {
                new Board { Id = 1, Name = "old name", CreationDate = DateTime.Now },
            };
            DbContext.Fill(boards);

            Board board = new Board { Id = 345, Name = "new name" };

            Assert.Throws<NotFoundException>(() => Manager.Update(board));
        }

        [Fact]
        public void BoardManager_Update_Throws_BoardAlreadyExist_If_Board_With_Given_Name_Exists()
        {
            List<Board> boards = new List<Board> {
                new Board { Id =1, Name = "old name", CreationDate = DateTime.Now},
                new Board { Id =3, Name = "old name 2", CreationDate = DateTime.Now },
            };
            DbContext.Fill(boards);

            Board board = new Board { Id = 3, Name = "old name" };

            Assert.Throws<AlreadyExistException>(() => Manager.Update(board));
        }

        [Fact]
        public void BoardManager_Update_Dont_Throw_BoardAlreadyExistException_If_Board_Name_Exist_In_Edited_Board()
        {
            List<Board> boards = new List<Board> {
                new Board { Id = 1, Name = "old name", CreationDate = DateTime.Now},
                new Board { Id = 3, Name = "old name 2", CreationDate = DateTime.Now },
            };
            DbContext.Fill(boards);

            Board board = boards.First();
            board.Description = "desc";

            Manager.Update(board);
        }

        [Fact]
        public void BoardManager_Throws_UnauthorizedAccess_If_Edit_Foreign_Board()
        {
            List<Board> boards = new List<Board> {
                new Board { Id = 1, CreatorId = "creatorId", Name = "old name", CreationDate = DateTime.Now},
            };
            DbContext.Fill(boards);

            Board board = new Board { Id = 1, CreatorId = "otherCreatorId", Description = "desc", Name = "old name 2" };

            Assert.Throws<UnauthorizedAccessException>(() => Manager.Update(board));
        }

        [Fact]
        public void BoardManager_Update_ThrowsInvalidOperationException_When_PrivateBoard_And_AcceptanceType_ExternalUsersOnly()
        {
            var board = new Board { Id = 1, CreatorId = "creatorId", Name = "name", CreationDate = DateTime.Now };
            List<Board> boards = new List<Board> { board };
            DbContext.Fill(boards);

            board.AcceptanceType = AcceptanceType.FromExternalUsersOnly;
            board.IsPrivate = true;

            Assert.Throws<ValidationException>(() => Manager.Update(board));
        }
    }
}
