using FluentAssertions;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Model;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Xunit;

namespace iKudo.Domain.Tests.Boards
{
    public class BoardManagerInviteTests : BoardManagerBaseTest
    {
        [Fact]
        public void BoardManagerInvite_UserEmails_CallsEmailSender()
        {
            Board board = new Board { CreatorId = "user", Id = 1 };
            DbContext.Fill(board);
            string userEmail = "user@email.com";
            string userEmail2 = "user@email.com";

            Manager.Invite("user", board.Id, new string[] { userEmail, userEmail2 });

            EmailSenderMock.Verify(x => x.SendAsync(It.IsAny<IEnumerable<MailMessage>>()));
        }

        [Fact]
        public void BoardManagerInvite_UserEmails_AddsInvitations()
        {
            Board board = new Board { CreatorId = "user", Id = 1 };
            DbContext.Fill(board);
            string userEmail = "user@email.com";
            string userEmail2 = "user@email.com";

            Manager.Invite("user", board.Id, new string[] { userEmail, userEmail2 });

            DbContext.BoardInvitations.FirstOrDefault(x => x.Email == userEmail).Should().NotBeNull();
            DbContext.BoardInvitations.FirstOrDefault(x => x.Email == userEmail2).Should().NotBeNull();
        }

        [Fact]
        public void BoardManagerInvite_InvitationForGivenEmailExists_SetsIsActiveFlagToFalseAndAddsNewInvitation()
        {
            string userEmail = "user@email.com";
            DbContext.Fill(CreateBoardInvitation(userEmail, 1, "creator"));

            Manager.Invite("creator", 1, new string[] { userEmail });

            var oldInvitation = DbContext.BoardInvitations.FirstOrDefault(x => !x.IsActive && x.Email == userEmail && x.BoardId == 1);
            oldInvitation.Should().NotBeNull();
            var newInvitation = DbContext.BoardInvitations.FirstOrDefault(x => x.IsActive && x.Email == userEmail && x.BoardId == 1);
            newInvitation.Should().NotBeNull();
        }

        [Fact]
        public void BoardManagerInvite_UserIsNotBoardOwner_UnauthorizedAccessExceptionThrown()
        {
            string userEmail = "user@email.com";
            Board board = new Board { CreatorId = "user", Id = 1 };
            DbContext.Fill(board);

            Manager.Invite("user", board.Id, new string[] { userEmail });
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await Manager.Invite("otherUser", board.Id, new string[] { userEmail }));
        }

        [Fact]
        public void BoardManagerInvite_UserInvitesHimself_InvalidOperationExceptionThrown()
        {
            List<UserBoard> userBoards = new List<UserBoard> { new UserBoard("user", 1) };
            var board = new Board { Id = 1, CreatorId = "user", UserBoards = userBoards };
            var user = new User { Id = "user", Email = "user@email.com", FirstName = "user", LastName = "asd", UserBoards = userBoards };
            DbContext.Fill(board);
            DbContext.Fill(user);

            Func<Task> invite = async () => await Manager.Invite("user", board.Id, new string[] { user.Email });

            invite.ShouldThrow<KudoException>();
        }

        private static BoardInvitation CreateBoardInvitation(string userEmail, int boardId, string creator)
        {
            return new BoardInvitation
            {
                BoardId = boardId,
                Board = new Board { Id = boardId, CreatorId = creator },
                Code = Guid.NewGuid(),
                CreationDate = DateTime.Now,
                CreatorId = creator,
                Email = userEmail,
                IsActive = true
            };
        }
    }
}
