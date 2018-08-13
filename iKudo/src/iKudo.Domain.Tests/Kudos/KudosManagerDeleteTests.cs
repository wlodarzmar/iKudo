using FluentAssertions;
using iKudo.Domain.Exceptions;
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
    }
}
