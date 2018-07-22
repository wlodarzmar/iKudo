using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Domain.Model;
using System;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Boards
{
    public class BoardManagerInviteAcceptTests : BoardManagerBaseTest
    {
        [Fact]
        public void BoardManager_AcceptInvitation_AddUserBoardEntry()
        {
            var board = new Board { Id = 1, };
            var invitation = new BoardInvitation { Board = board, BoardId = board.Id, Code = Guid.NewGuid() };
            DbContext.Fill(invitation);

            Manager.AcceptInvitation("user", board.Id, invitation.Code.ToString());

            DbContext.UserBoards.FirstOrDefault(x => x.BoardId == board.Id && x.UserId == "user").Should().NotBeNull();
        }

        [Fact]
        public void BoardManager_AcceptInvitation_InvitationSenderReceivesNotificationWhenInvitationAccepted()
        {
            var board = new Board { Id = 1, CreatorId = "creator" };
            var invitation = new BoardInvitation { Board = board, BoardId = board.Id, Code = Guid.NewGuid() };
            DbContext.Fill(invitation);

            Manager.AcceptInvitation("user", board.Id, invitation.Code.ToString());

            DbContext.Notifications.FirstOrDefault(x => x.ReceiverId == board.CreatorId &&
                                                        x.SenderId == "user" &&
                                                        x.Type == NotificationTypes.BoardInvitationAccepted &&
                                                        x.BoardId == board.Id)
                                   .Should().NotBeNull();
        }

        [Fact]
        public void BoardManager_AcceptInvitation_ThrowsInvalidOperationExceptionWhenInvitationDoesntExist()
        {
            var board = new Board { Id = 1 };
            DbContext.Fill(board);

            Assert.Throws<InvalidOperationException>(() => Manager.AcceptInvitation("user", board.Id, "otherCode"));
        }

        [Fact]
        public void BoardManager_AcceptInvitationWhenUserIsAlreadyMemberOfBoard_InvalidOperationExceptionThrown()
        {
            var board = new Board { Id = 1, CreatorId = "creator" };
            board.UserBoards.Add(new UserBoard("user", board.Id));
            var invitation = new BoardInvitation { Board = board, BoardId = board.Id, Code = Guid.NewGuid() };
            DbContext.Fill(invitation);

            Assert.Throws<InvalidOperationException>(() => Manager.AcceptInvitation("user", board.Id, invitation.Code.ToString()));
        }
    }
}
