using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Linq;

namespace iKudo.Clients.Web.UITests
{
    interface ICanLog
    {
        void Log(string login, string password);
    }

    internal class GoggleProviderLog : ICanLog
    {
        private readonly RemoteWebDriver driver;

        public GoggleProviderLog(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        public void Log(string login, string password)
        {
            var googleBtn = driver.WaitForElements(By.ClassName("auth0-lock-social-button"))
                .First(x => x.GetAttribute("data-provider") == "google-oauth2");
            googleBtn.Click();

            var emailInp = driver.WaitForElement(By.Id("identifierId"));
            emailInp.SendKeys(login);
            emailInp.SendKeys(Keys.Enter);

            var passInp = driver.WaitForElement(By.Name("password"));
            passInp.SendKeys(password);
            passInp.SendKeys(Keys.Enter);
        }
    }
}
