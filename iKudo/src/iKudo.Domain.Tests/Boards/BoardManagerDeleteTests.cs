using iKudo.Domain.Exceptions;
using iKudo.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests
{
    public class BoardManagerDeleteTests : BoardManagerBaseTest
    {
        //TODO: rename
        [Fact]
        public void CompanyManager_Delete_Removes_Board()
        {
            int companyId = 1;
            string creatorId = "creator";
            var data = new List<Board> { new Board { Id = companyId, Name = "company name", CreatorId = creatorId } };
            DbContext.Fill(data);

            Manager.Delete(creatorId, companyId);

            Assert.Equal(0, DbContext.Boards.Count());
        }

        [Fact]
        public void Company_Delete_Throws_NotFoundException_If_NotExist()
        {
            var data = new List<Board> { new Board { Id = 1, Name = "company name" } };
            DbContext.Fill(data);

            Assert.Throws<NotFoundException>(() => Manager.Delete(It.IsAny<string>(), 2));
        }

        [Fact]
        public void Company_Delete_Throws_UnauthorizedAccessException_If_Try_Delete_Foreign_Board()
        {
            var data = new List<Board> { new Board { CreatorId = "12345", Id = 1, Name = "company name" } };
            DbContext.Fill(data);

            Assert.Throws<UnauthorizedAccessException>(() => Manager.Delete("fakeUserId", 1));
        }
    }
}
