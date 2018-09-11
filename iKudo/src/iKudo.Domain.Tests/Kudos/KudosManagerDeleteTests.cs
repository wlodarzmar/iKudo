using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Domain.Exceptions;
using iKudo.Domain.Model;
using iKudo.Domain.Tests.Helpers;
using Moq;
using System;
using System.Linq;
using Xunit;
namespace iKudo.Domain.Tests.Kudos
{
    public class KudosManagerDeleteTests : KudosManagerBaseTest
    {
        [Fact]
        public void KudosManagerDelete_KudoSender_RemovesKudoFromBoard()
        {
            var kudo = KudosHelper.CreateKudo(1);
            DbContext.Fill(kudo);

            Manager.Delete(kudo.SenderId, kudo.Id);

            DbContext.Kudos.FirstOrDefault(x => x.Id == kudo.Id).Should().BeNull();
        }

        [Fact]
        public void KudosManagerDelete_KudoDoesntExist_ThrowsNotFoundException()
        {
            Action deleteAction = () => Manager.Delete("user", 1);

            deleteAction.ShouldThrow<NotFoundException>();
        }

        [Fact]
        public void KudosManagerDelete_BoardCreator_CanRemoveKudo()
        {
            var kudo = KudosHelper.CreateKudo(1);
            DbContext.Fill(kudo);

            Manager.Delete(kudo.Board.CreatorId, kudo.Id);

            DbContext.Kudos.FirstOrDefault(x => x.Id == kudo.Id).Should().BeNull();
        }

        [Fact]
        public void KudosManagerDelete_NeitherKudoSenderNorBoardCreator_ThrowsUnauthorizedAccessException()
        {
            var kudo = KudosHelper.CreateKudo(1);
            DbContext.Fill(kudo);

            Action deleteAction = () => Manager.Delete("someOtherUserId", kudo.Id);

            deleteAction.ShouldThrow<UnauthorizedAccessException>();
        }

        [Fact]
        public void KudosManagerDelete_KudosWithImage_CallsFileServiceDelete()
        {
            var kudo = KudosHelper.CreateKudo(1);
            kudo.Image = "pathToImage.ext";
            DbContext.Fill(kudo);

            Manager.Delete(kudo.SenderId, kudo.Id);

            FileStorageMock.Verify(x => x.Delete(It.Is<string>(path => path == kudo.Image)));
        }

        [Fact]
        public void KudosManagerDelete_KudosWithNotification_NotificationDeletedAsWell()
        {
            var board = BoardHelper.CreateBoard();
            var kudo = KudosHelper.CreateKudo(board, "sender", "receiver", true);
            var notification = new Notification
            {
                BoardId = board.Id,
                Board = board,
                Kudo = kudo,
                KudoId = kudo.Id,
                CreationDate = DateTime.Now,
                ReceiverId = kudo.ReceiverId,
                SenderId = board.CreatorId,
                Type = NotificationTypes.KudoAccepted
            };
            DbContext.Fill(kudo);
            DbContext.Fill(notification);

            Manager.Delete(board.CreatorId, kudo.Id);

            DbContext.Notifications.FirstOrDefault(x => x.Type == NotificationTypes.KudoAccepted).Should().BeNull();
        }
    }
}
