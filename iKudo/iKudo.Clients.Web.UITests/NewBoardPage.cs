using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace iKudo.Clients.Web.UITests
{
    internal class NewBoardPage
    {
        private readonly RemoteWebDriver driver;

        public NewBoardPage(RemoteWebDriver driver)
        {
            this.driver = driver;
            NameInput = driver.WaitForElement(By.Id("board_name"));
            DescriptionInput = driver.WaitForElement(By.Id("board_description"));
            AddBoardBtn = driver.WaitForElement(By.Id("add_board"));
        }

        public IWebElement NameInput { get; set; }
        public string Name
        {
            get
            {
                return NameInput.Text;
            }
            set
            {
                NameInput.SendKeys(value);
            }
        }

        public IWebElement DescriptionInput { get; set; }
        public string Description
        {
            get
            {
                return DescriptionInput.Text;
            }
            set
            {
                DescriptionInput.SendKeys(value);
            }
        }

        public IWebElement AddBoardBtn { get; private set; }
    }
}