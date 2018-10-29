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

        internal NewBoardPage Name(string name)
        {
            NameInput.SendKeys(name);

            return this;
        }

        internal NewBoardPage Description(string description)
        {
            DescriptionInput.SendKeys(description);

            return this;
        }

        internal void Add()
        {
            AddBoardBtn.Click();
        }

        private IWebElement NameInput { get; set; }
        private string BoardName
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

        private IWebElement DescriptionInput { get; set; }
        private string BoardDescription
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

        private IWebElement AddBoardBtn { get; set; }
    }
}