using NUnit.Framework;
using System.Threading;

namespace iKudo.Clients.Web.AutomaticTests
{
    [TestFixture]
    public class LoginTests : TestBase
    {

        [Test]
        public void MyMethod()
        {
            Browser.Visit(Root);
            
            Browser.WaitForElementById("login_btn").Click();
            Thread.Sleep(3000);
            Browser.ClickLink("Sign Up");
            Browser.FindXPath("//input[@name='email']").FillInWith("test@text.com");
            Browser.FindXPath("//input[@name='password']").FillInWith("testTEST123");
            Thread.Sleep(1000);
            Browser.FindXPath("//button[@class='auth0-lock-submit']").Click();

            Thread.Sleep(20000);
        }
    }
}
