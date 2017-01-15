using Coypu;
using System.Threading;

namespace iKudo.Clients.Web.AutomaticTests.TestHelpers
{
    public class AccountHelper
    {
        public AccountHelper(BrowserSession browser, string root)
        {
            Root = root;
            Browser = browser;
        }

        private string Root { get; set; }

        private BrowserSession Browser { get; set; }

        public void RegisterUser(string email, string password)
        {
            Browser.Visit(Root);
            Browser.WaitForElementById("login_btn").Click();
            Browser.WaitForLink("Sign Up").Click();
            Browser.FindXPath("//input[@name='email']").FillInWith(email);
            Browser.FindXPath("//input[@name='password']").FillInWith(password);
            Browser.FindXPath("//button[@class='auth0-lock-submit']").Click();
        }

        public void Logout()
        {
            Browser.WaitForElementById("logout_btn").Click();
        }

        public void Login(string email, string password)
        {
            Browser.WaitForElementById("login_btn").Click();
            Browser.WaitForLink("Not your account?").Click();

            Browser.WaitForElementByXpath("//input[@name='email']").FillInWith(email);
            Browser.WaitForElementByXpath("//input[@name='password']").FillInWith(password);

            Browser.FindXPath("//button[@class='auth0-lock-submit']").Click();
        }
    }
}
