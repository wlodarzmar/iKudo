using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Model;
using iKudo.Domain.Tests.Helpers;
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
            var board = BoardHelper.CreateBoard();
            var invitation = BoardHelper.CreateBoardInvitation(board);
            DbContext.Fill(invitation);

            Manager.AcceptInvitation("user", board.Id, invitation.Code.ToString());

            DbContext.UserBoards.FirstOrDefault(x => x.BoardId == board.Id && x.UserId == "user").Should().NotBeNull();
        }

        [Fact]
        public void BoardManager_AcceptInvitation_InvitationSenderReceivesNotificationWhenInvitationAccepted()
        {
            var board = BoardHelper.CreateBoard();
            var invitation = BoardHelper.CreateBoardInvitation(board);
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
            var board = BoardHelper.CreateBoard();
            board.UserBoards.Add(new UserBoard("user", board.Id));
            var invitation = BoardHelper.CreateBoardInvitation(board);
            DbContext.Fill(invitation);

            Assert.Throws<InvalidOperationException>(() => Manager.AcceptInvitation("user", board.Id, invitation.Code.ToString()));
        }

        [Fact]
        public void BoardManager_AcceptInvitation_BoardInvitationIsNotActive_ThrowsKudoException()
        {
            var board = BoardHelper.CreateBoard();
            var invitation = BoardHelper.CreateBoardInvitation(board);
            invitation.IsActive = false;
            DbContext.Fill(invitation);

            Action secondInvitation = () => Manager.AcceptInvitation("user", board.Id, invitation.Code.ToString());

            secondInvitation.Should().Throw<KudoException>();
        }

        [Fact]
        public void BoardManager_AcceptInvitation_SetsIsActiveFlagToFalse()
        {
            var board = BoardHelper.CreateBoard();
            var invitation = BoardHelper.CreateBoardInvitation(board);
            DbContext.Fill(invitation);

            Manager.AcceptInvitation("user", board.Id, invitation.Code.ToString());

            var acceptedInvitation = DbContext.BoardInvitations.First(x => x.Id == invitation.Id);
            acceptedInvitation.IsActive.Should().BeFalse();
        }
    }
}
