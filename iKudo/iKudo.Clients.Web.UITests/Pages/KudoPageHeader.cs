using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System.Linq;
using System.Threading;

namespace iKudo.Clients.Web.UITests.Pages
{
    public class KudoPageHeader
    {
        private readonly ICanLog log;

        public KudoPageHeader(RemoteWebDriver driver)
        {
            Driver = driver;
            log = new DatabaseAuth0Log(driver);
        }

        public RemoteWebDriver Driver { get; }

        public IWebElement BoardsMenuItem { get; private set; }

        internal void Log(string login, string password)
        {
            var link = Driver.WaitForElement(By.Id("login_btn"));
            link.Click();

            log.Log(login, password);

            Driver.WaitForElement(By.Id("logged_as")).Text.Should().NotBeNullOrWhiteSpace();

            InitMenu();
        }

        internal BoardsPage GoToBoards()
        {
            Thread.Sleep(1000);
            BoardsMenuItem.Click();
            return new BoardsPage(Driver);
        }

        private void InitMenu()
        {
            BoardsMenuItem = Driver.WaitForElement(By.ClassName("navbar-nav"))
                                   .WaitForElements(By.TagName("a"))
                                   .First(x => x.GetAttribute("href").Contains("/boards"));
        }
    }
}
