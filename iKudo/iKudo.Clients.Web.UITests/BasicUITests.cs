using iKudo.Clients.Web.UITests.Pages;
using System;
using System.Threading;
using Xunit;

namespace iKudo.Clients.Web.UITests
{
    public class BasicUITests : KudoPage
    {
        [Fact]
        public void BasicPath()
        {
            Driver.Navigate().GoToUrl(KudoConfiguration.KudoPageUrl);
            Header.Log(KudoConfiguration.User1Email, KudoConfiguration.User1Password);

            var newBoardPage = Header.GoToBoards().NewBoard();
            string boardName = $"{Faker.Company.Name()}: {Guid.NewGuid()}";
            newBoardPage.Name(boardName)
                .Description(Faker.Lorem.Paragraph())
                .Add();

            BoardDetailsPage boardDetailsPage = Header.GoToBoards().Board(boardName).Details();

            using (var tempMail = new TempMailPage())
            using (var acceptInvitationPage = new InvitationAcceptancePage())
            {
                var email = tempMail.GetMail();
                boardDetailsPage.InviteEmail(email)
                    .SendInvitations();

                var link = tempMail.GetInvitationLink();
                acceptInvitationPage.Driver.Navigate().GoToUrl(link);
                acceptInvitationPage.Log(KudoConfiguration.User2Email, KudoConfiguration.User2Password);
                acceptInvitationPage.AcceptInvitation();
                Thread.Sleep(5000);
            }

            var boardPreview = Header.GoToBoards().Board(boardName).Preview();
            var newKudoPage = boardPreview.AddKudo();

            newKudoPage.SelectKudoType(1)
                .SelectReceiver(1)
                .Content(Faker.Lorem.Paragraph())
                .Add();

            Thread.Sleep(1000);
        }
    }
}