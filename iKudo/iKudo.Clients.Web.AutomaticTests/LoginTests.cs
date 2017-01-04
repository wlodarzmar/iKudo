using FluentAssertions;
using iKudo.Clients.Web.AutomaticTests.TestHelpers;
using NUnit.Framework;
using System;
using System.Drawing.Imaging;
using System.Threading;

namespace iKudo.Clients.Web.AutomaticTests
{
    [TestFixture]
    public class LoginTests : TestBase
    {
        private const string password = "1qazXSW@3edc";
        private AccountHelper accountHelper;

        //public LoginTests()
        //{
        //}

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            accountHelper = new AccountHelper(Browser, Root);
        }

        [Test]
        public void UserCanRegister()
        {
            string email = $"{Guid.NewGuid().ToString()}@test.com";

            accountHelper.RegisterUser(email, password);

            string loggedAsText = Browser.WaitForElementById("logged_as").Text;
            loggedAsText.Should().Contain(email);
        }

        [Test]
        public void UserCanLogoutAndLogin()
        {
            string email = $"{Guid.NewGuid().ToString()}@test.com";

            accountHelper.RegisterUser(email, password);
            accountHelper.Logout();
            accountHelper.Login(email, password);

            string loggedAsText = Browser.WaitForElementById("logged_as").Text;
            loggedAsText.Should().Contain(email);
        }

    }
}
