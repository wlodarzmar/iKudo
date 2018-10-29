using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace iKudo.Clients.Web.UITests.Pages
{
    internal class BoardDetailsPage
    {
        private RemoteWebDriver driver;

        public BoardDetailsPage(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        public BoardDetailsPage InviteEmail(string email)
        {
            var emailInp = driver.WaitForElement(By.Id("invite_email"));
            emailInp.SendKeys(email);
            emailInp.SendKeys(Keys.Enter);

            return this;
        }

        internal void SendInvitations()
        {
            driver.WaitForElement(By.Id("send_invitations")).Click();
        }
    }
}