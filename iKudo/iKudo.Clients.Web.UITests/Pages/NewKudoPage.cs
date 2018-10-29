using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace iKudo.Clients.Web.UITests.Pages
{
    internal class NewKudoPage
    {
        private RemoteWebDriver driver;

        public NewKudoPage(RemoteWebDriver driver)
        {
            this.driver = driver;
        }

        internal NewKudoPage SelectKudoType(int typeIndex)
        {
            var kudoTypeSelect = new SelectElement(driver.WaitForElement(By.Id("kudoTypeSelect")));
            kudoTypeSelect.SelectByIndex(typeIndex);

            return this;
        }

        internal NewKudoPage SelectReceiver(int receiverIndex)
        {
            var receiverSelect = new SelectElement(driver.WaitForElement(By.Id("receiverSelect")));
            receiverSelect.SelectByIndex(receiverIndex);

            return this;
        }

        internal NewKudoPage Content(string text)
        {
            driver.WaitForElement(By.Id("kudo_content")).SendKeys(text);

            return this;
        }

        internal void Add()
        {
            driver.WaitForElement(By.Id("add_kudo")).Click();
        }
    }
}