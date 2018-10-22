using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace iKudo.Clients.Web.UITests
{
    internal class BoardDetailsPage
    {
        private RemoteWebDriver driver;
        private string name;

        public BoardDetailsPage(RemoteWebDriver driver, string name)
        {
            this.driver = driver;
            this.name = name;
        }

        public IWebElement SendInvitationsBtn
        {
            get
            {
                return driver.WaitForElement(By.Id("add_board"));
            }
        }

        public void AddInviteEmail(string email)
        {
            var emailInp = driver.WaitForElement(By.Id("invite_email"));
            emailInp.SendKeys(email);
            emailInp.SendKeys(Keys.Enter);
        }
    }
}