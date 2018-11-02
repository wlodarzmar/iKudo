using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace iKudo.Clients.Web.UITests
{
    internal class DatabaseAuth0Log : ICanLog
    {
        private readonly RemoteWebDriver driver;

        public DatabaseAuth0Log(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        public void Log(string login, string password)
        {
            driver.WaitForElement(By.Name("email")).SendKeys(login);
            IWebElement passwordInput = driver.WaitForElement(By.Name("password"));
            passwordInput.SendKeys(password);
            passwordInput.SendKeys(Keys.Enter);
        }
    }
}
