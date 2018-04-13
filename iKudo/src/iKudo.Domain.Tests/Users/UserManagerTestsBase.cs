using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;

namespace iKudo.Domain.Tests.Users
{
    public class UserManagerTestsBase : BaseTest
    {
        public UserManagerTestsBase()
        {
            UserManager = new UserManager(DbContext);
        }

        public IManageUsers UserManager { get; set; }


        protected User CreateUser()
        {
            return new User { Id = "Id", FirstName = "FName", LastName = "LName", Email = "Email" };
        }
    }
}
