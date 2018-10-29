using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System.Linq;
using System.Threading;

namespace iKudo.Clients.Web.UITests.Pages
{
    internal class BoardListItem
    {
        private readonly RemoteWebDriver driver;
        private readonly string boardName;

        public BoardListItem(RemoteWebDriver driver, string boardName)
        {
            this.driver = driver;
            this.boardName = boardName;
        }

        public BoardDetailsPage Details()
        {
            var board = driver.WaitForElement(By.LinkText(boardName));
            var boardItem = board.FindParentByClassName("list-item");

            Actions action = new Actions(driver);
            action.MoveToElement(board).Perform();

            Thread.Sleep(1000);

            boardItem.WaitForElements(By.TagName("a"))
                .First(x => x.GetAttribute("href").Contains("details"))
                .Click();

            return new BoardDetailsPage(driver);
        }

        internal BoardPreview Preview()
        {
            driver.WaitForElement(By.LinkText(boardName)).Click();

            return new BoardPreview(driver, boardName);
        }
    }
}