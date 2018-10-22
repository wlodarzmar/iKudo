using OpenQA.Selenium;
using System;
using System.Diagnostics;
using Xunit;

namespace iKudo.Clients.Web.UITests
{
    public partial class UnitTest1 : KudoPage
    {
        [Fact]
        public void BasicPath()
        {
            Driver.Navigate().GoToUrl(Configuration["KudoWebUrl"]);
            Header.Log(Configuration["AppSettings:User1Email"], Configuration["AppSettings:User1Password"]);
            Header.BoardsMenuItem.Click();

            var newBoardPage = Header.Boards().NewBoard();
            string boardName = $"test board: {Guid.NewGuid()}";
            newBoardPage.Name = boardName;
            newBoardPage.Description = "test board description";
            newBoardPage.AddBoardBtn.Click();

            BoardsPage boards = Header.Boards();
            BoardDetailsPage boardDetailsPage = boards.Edit(boardName);

            using (var tempMailDriver = GetDriver())
            {
                tempMailDriver.Navigate().GoToUrl("https://temp-mail.org/en/");
                var email = tempMailDriver.WaitForElement(By.Id("mail")).GetAttribute("value");
                Debug.WriteLine("Pobrano maila");
                boardDetailsPage.AddInviteEmail(email);
                boardDetailsPage.SendInvitationsBtn.Click();
                Debug.WriteLine("Wys³ano zaproszenie");
                tempMailDriver.WaitForElement(By.LinkText("Invitation to kudo board"), 50).Click();
                var link = tempMailDriver.WaitForElement(By.PartialLinkText("mwlodarz.hostingasp.pl")).GetAttribute("href");

                //wyci¹gn¹æ logowanie googla do klaski

                tempMailDriver.Navigate().GoToUrl(link);
                tempMailDriver.WaitForElement(By.LinkText("Zaloguj")).Click();

                ICanLog googleLog = new GoggleProviderLog(tempMailDriver);
                googleLog.Log(Configuration["AppSettings:User2Email"], Configuration["AppSettings:User2Password"]);

                tempMailDriver.WaitForElement(By.Id("accept_invitation_btn")).Click();
            }

            boards = Header.Boards();
            Driver.WaitForElement(By.LinkText(boardName)).Click();
            Driver.WaitForElement(By.Id("add_kudo")).Click();
        }
    }
}
