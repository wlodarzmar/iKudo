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
    public class BoardManagerDeleteTests : BoardTestsBase
    {
        [Fact]
        public void CompanyManager_Delete_Removes_Board()
        {
            int companyId = 1;
            string creatorId = "creator";
            var data = new List<Board> { new Board { Id = companyId, Name = "company name", CreatorId = creatorId } };
            FillContext(data);
            IBoardManager manager = new BoardManager(DbContext);

            manager.Delete(creatorId, companyId);

            Assert.Equal(0, DbContext.Boards.Count());
        }

        [Fact]
        public void Company_Delete_Throws_NotFoundException_If_NotExist()
        {
            var data = new List<Board> { new Board { Id = 1, Name = "company name" } };
            FillContext(data);
            IBoardManager manager = new BoardManager(DbContext);

            Assert.Throws<NotFoundException>(() => manager.Delete(It.IsAny<string>(), 2));
        }

        [Fact]
        public void Company_Delete_Throws_UnauthorizedAccessException_If_Try_Delete_Foreign_Board()
        {
            var data = new List<Board> { new Board { CreatorId = "12345", Id = 1, Name = "company name" } };
            FillContext(data);
            IBoardManager manager = new BoardManager(DbContext);

            Assert.Throws<UnauthorizedAccessException>(() => manager.Delete("fakeUserId", 1));
        }
    }
}
