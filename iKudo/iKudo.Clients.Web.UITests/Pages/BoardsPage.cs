using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System.Linq;
using System.Threading;

namespace iKudo.Clients.Web.UITests.Pages
{
    internal class BoardsPage
    {
        private readonly RemoteWebDriver driver;

        public BoardsPage(RemoteWebDriver driver)
        {
            this.driver = driver;
            CreateBoardBtn = driver.WaitForElement(By.Id("add_board"));
        }

        public IWebElement CreateBoardBtn { get; private set; }

        internal NewBoardPage NewBoard()
        {
            CreateBoardBtn.Click();
            return new NewBoardPage(driver);
        }

        internal BoardDetailsPage Details(string name)
        {
            var board = driver.WaitForElement(By.LinkText(name));


            var boardItem = board.FindParentByClassName("list-item");


            Actions action = new Actions(driver);
            action.MoveToElement(board).Perform();
            Thread.Sleep(1000);

            boardItem.WaitForElements(By.TagName("a"))
                .First(x => x.GetAttribute("href").Contains("details"))
                .Click();

            return new BoardDetailsPage(driver);
        }

        internal BoardListItem Board(string boardName)
        {
            return new BoardListItem(driver, boardName);
        }
    }
}
