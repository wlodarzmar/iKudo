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
            return CreateUser("Id", "fname");
        }

        protected User CreateUser(string id, string firstName)
        {
            return new User { Id = id, FirstName = firstName, LastName = "lname", Email = "email" };
        }
    }
}
