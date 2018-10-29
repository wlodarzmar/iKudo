using OpenQA.Selenium;

namespace iKudo.Clients.Web.UITests.Pages
{
    internal class InvitationAcceptancePage : KudoPage
    {
        internal void Log(string user2Email, string user2Password)
        {
            Driver.WaitForElement(By.LinkText("Zaloguj")).Click();
            ICanLog googleLog = new GoggleProviderLog(Driver);
            googleLog.Log(KudoConfiguration.User2Email, KudoConfiguration.User2Password);
        }

        internal void AcceptInvitation()
        {
            Driver.WaitForElement(By.Id("accept_invitation_btn")).Click();
        }
    }
}