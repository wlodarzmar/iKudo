using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace iKudo.Clients.Web.UITests
{
    internal class BoardPreview
    {
        private RemoteWebDriver driver;
        private string boardName;

        public BoardPreview(RemoteWebDriver driver, string boardName)
        {
            this.driver = driver;
            this.boardName = boardName;
        }

        public NewKudoPage AddKudo()
        {
            driver.WaitForElement(By.Id("add_kudo")).Click();

            return new NewKudoPage(driver);
        }
    }
}