using OpenQA.Selenium;

namespace iKudo.Clients.Web.UITests.Pages
{
    internal class TempMailPage : KudoPage
    {
        private const int GetMailMaxAttempts = 20;
        private const int GetInvitationLinkMaxAttempts = 50;

        public TempMailPage()
        {
            Driver.Navigate().GoToUrl(KudoConfiguration.TempMailUrl);
        }

        internal string GetMail()
        {
            return Driver.WaitForElement(By.Id("mail"), GetMailMaxAttempts).GetAttribute("value");
        }

        internal string GetInvitationLink()
        {
            Driver.WaitForElement(By.LinkText("Invitation to kudo board"), GetInvitationLinkMaxAttempts).Click();
            var link = Driver.WaitForElement(By.PartialLinkText(KudoConfiguration.KudoPageUrl)).GetAttribute("href");

            return link;
        }
    }
}