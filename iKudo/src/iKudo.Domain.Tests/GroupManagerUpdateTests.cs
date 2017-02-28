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
    public class GroupManagerUpdateTests : GroupTestsBase
    {
        [Fact]
        public void GroupManager_Throws_ArgumentNullException_If_Group_Is_Null()
        {
            Mock<KudoDbContext> kudoContextMock = new Mock<KudoDbContext>();
            IGroupManager manager = new GroupManager(kudoContextMock.Object);

            Assert.Throws(typeof(ArgumentNullException), () => manager.Update(null));
        }

        [Fact]
        public void GroupManager_Update_Assign_New_ModificationDate()
        {
            Mock<KudoDbContext> kudoContextMock = new Mock<KudoDbContext>();
            DateTime oldDate = DateTime.Now.AddDays(-5);
            List<Group> groups = new List<Group> {
                new Group { Id =1, Name = "old name", ModificationDate = oldDate, CreationDate = oldDate },
            };
            kudoContextMock.Setup(x => x.Groups).Returns(ConfigureCompaniesMock(groups.AsQueryable()).Object);

            IGroupManager manager = new GroupManager(kudoContextMock.Object);

            Group group = new Group { Id = 1, ModificationDate = oldDate, Name = "new name" };
            manager.Update(group);

            Assert.True(group.ModificationDate > oldDate);
        }

        [Fact]
        public void GroupManager_Update_Calls_Companies_Update_And_SaveChanges_Once()
        {
            Mock<KudoDbContext> kudoContextMock = new Mock<KudoDbContext>();
            DateTime oldDate = DateTime.Now.AddDays(-5);
            List<Group> groups = new List<Group> {
                new Group { Id =1, Name = "old name", ModificationDate = oldDate, CreationDate = oldDate },
            };
            var groupsMock = ConfigureCompaniesMock(groups.AsQueryable());
            kudoContextMock.Setup(x => x.Groups).Returns(groupsMock.Object);

            IGroupManager manager = new GroupManager(kudoContextMock.Object);

            Group group = new Group { Id = 1, ModificationDate = oldDate, Name = "new name" };
            manager.Update(group);

            kudoContextMock.Verify(x => x.Update(It.Is<Group>(g => g == group)), Times.Once);
            kudoContextMock.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void GroupManager_Update_Throws_NotFoundException_If_Group_Not_Exist()
        {
            Mock<KudoDbContext> kudoContextMock = new Mock<KudoDbContext>();
            List<Group> groups = new List<Group> {
                new Group { Id = 1, Name = "old name", CreationDate = DateTime.Now },
            };
            var groupsMock = ConfigureCompaniesMock(groups.AsQueryable());
            kudoContextMock.Setup(x => x.Groups).Returns(groupsMock.Object);

            IGroupManager manager = new GroupManager(kudoContextMock.Object);

            Group group = new Group { Id = 345, Name = "new name" };

            Assert.Throws(typeof(NotFoundException), () => manager.Update(group));
        }

        [Fact]
        public void GroupManager_Update_Throws_GroupAlreadyExist_If_Group_With_Given_Name_Exists()
        {
            Mock<KudoDbContext> kudoContextMock = new Mock<KudoDbContext>();
            List<Group> groups = new List<Group> {
                new Group { Id =1, Name = "old name", CreationDate = DateTime.Now},
                new Group { Id =3, Name = "old name 2", CreationDate = DateTime.Now },
            };
            var groupsMock = ConfigureCompaniesMock(groups.AsQueryable());
            kudoContextMock.Setup(x => x.Groups).Returns(groupsMock.Object);

            IGroupManager manager = new GroupManager(kudoContextMock.Object);

            Group group = new Group { Id = 3, Name = "old name" };

            Assert.Throws(typeof(GroupAlreadyExistException), () => manager.Update(group));
        }

        [Fact]
        public void GroupManager_Update_Dont_Throw_GroupAlreadyExistException_If_Group_Name_Exist_In_Edited_Group()
        {
            Mock<KudoDbContext> kudoContextMock = new Mock<KudoDbContext>();
            List<Group> groups = new List<Group> {
                new Group { Id = 1, Name = "old name", CreationDate = DateTime.Now},
                new Group { Id = 3, Name = "old name 2", CreationDate = DateTime.Now },
            };
            var groupsMock = ConfigureCompaniesMock(groups.AsQueryable());
            kudoContextMock.Setup(x => x.Groups).Returns(groupsMock.Object);

            IGroupManager manager = new GroupManager(kudoContextMock.Object);

            Group group = new Group { Id = 3, Description = "desc", Name = "old name 2" };

            manager.Update(group);
        }

        [Fact]
        public void GroupManager_Throws_UnauthorizedAccess_If_Edit_Foreign_Group()
        {
            Mock<KudoDbContext> kudoContextMock = new Mock<KudoDbContext>();
            List<Group> groups = new List<Group> {
                new Group { Id = 1, CreatorId = "creatorId", Name = "old name", CreationDate = DateTime.Now},
            };
            var groupsMock = ConfigureCompaniesMock(groups.AsQueryable());
            kudoContextMock.Setup(x => x.Groups).Returns(groupsMock.Object);
            IGroupManager manager = new GroupManager(kudoContextMock.Object);

            Group group = new Group { Id = 1, CreatorId = "otherCreatorId", Description = "desc", Name = "old name 2" };

            Assert.Throws(typeof(UnauthorizedAccessException), () => manager.Update(group));
        }
    }
}
