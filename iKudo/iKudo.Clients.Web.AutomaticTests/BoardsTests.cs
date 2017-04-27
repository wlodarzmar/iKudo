using iKudo.Clients.Web.AutomaticTests.TestHelpers;
using NUnit.Framework;
using System;

namespace iKudo.Clients.Web.AutomaticTests
{
    [TestFixture]
    public class BoardsTests : ViewTestBase
    {
        private const string password = "1qazXSW@3edc";
        private AccountHelper accountHelper;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            accountHelper = new AccountHelper(Browser, Root);
        }

        [Test]
        public void Boards_Add_Edit_CantAddWithSameName_PreviewDetails_Delete()
        {
            string email = $"{Guid.NewGuid().ToString()}@test.com";
            accountHelper.RegisterUser(email, password);
            accountHelper.Logout();
            accountHelper.Login(email, password);

            Browser.WaitForLink("Add Board").Click();
            Browser.WaitForElementById("add_board");
            string boardName = "Board name: " + Guid.NewGuid().ToString();
            Browser.FillIn("board_name").With(boardName);
            string boardDescription = "DESC: " + Guid.NewGuid().ToString();
            Browser.FillIn("board_description").With(boardDescription);

            Browser.WaitForElementById("add_board").Click();
            Browser.WaitForDialog("dodano tablice").AcceptModalDialog();

            Browser.WaitForLink("Tablice").Click();
            Browser.WaitForText(boardName);
            Assert.That(Browser.HasContent(boardName));
            Assert.That(Browser.HasContent(boardDescription));

            Browser.FindXPath($"//text()[contains(.,'{boardName}')]/ancestor::li/div[2]/a[text()[contains(.,'Edytuj')]]").Click();
            Browser.WaitForText("Edytuj grupę");

            string newBoardName = "New Board name: " + Guid.NewGuid().ToString();
            Browser.FillIn("board_name").With(newBoardName);
            string newBoardDescription = "New DESC: " + Guid.NewGuid().ToString();
            Browser.FillIn("board_description").With(newBoardDescription);
            Browser.FindId("edit_board").Click();
            Browser.WaitForDialog("zapisano tablice").AcceptModalDialog();

            Browser.WaitForLink("Tablice").Click();
            Browser.WaitForText(newBoardName);
            Assert.That(Browser.HasContent(newBoardName));
            Assert.That(Browser.HasContent(newBoardDescription));

            Browser.WaitForLink("Add Board").Click();
            Browser.FillIn("board_name").With(newBoardName);
            Browser.FillIn("board_description").With(newBoardDescription);
            Browser.FindId("add_board").Click();
            Browser.WaitForDialog($"Board '{newBoardName}' already exists").AcceptModalDialog();

            Browser.WaitForLink("Tablice").Click();
            Browser.FindXPath($"//text()[contains(.,'{newBoardName}')]/ancestor::li/div[2]/a[text()[contains(.,'Szczegóły')]]").Click();
            Browser.WaitForText(newBoardName);

            Assert.That(Browser.HasContent(newBoardName));
            Assert.That(Browser.HasContent(newBoardDescription));
            Assert.That(Browser.HasContent(email));

            Browser.WaitForLink("Tablice").Click();
            Browser.FindXPath($"//text()[contains(.,'{newBoardName}')]/ancestor::li/div[2]/button[text()[contains(.,'Usuń')]]").Click();
            Browser.WaitForDialog("Usunięto tablice").AcceptModalDialog();

            Assert.That(Browser.HasNoContent(newBoardName));
            Assert.That(Browser.HasNoContent(newBoardDescription));
        }
    }
}
