using FluentAssertions;
using iKudo.Domain.Enums;
using iKudo.Domain.Tests.Helpers;
using System;
using System.Linq;
using Xunit;

namespace iKudo.Domain.Tests.Kudos
{
    public class KudosManagerAcceptKudosTests : KudosManagerBaseTest
    {
        [Fact]
        public void AcceptKudo_NewKudo_KudoAccepted()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            var acceptedKudo = Manager.Accept(userId, kudo.Id);

            acceptedKudo.Status.Should().Be(KudoStatus.Accepted);
        }

        [Fact]
        public void RejectKudo_NewKudo_KudoRejected()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            var acceptedKudo = Manager.Reject(userId, kudo.Id);

            acceptedKudo.Status.Should().Be(KudoStatus.Rejected);
        }

        [Fact]
        public void RejectKudo_UserIsNotOwnerOfBoard_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, "boardCreator", "sender", "receiver", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Reject(userId, kudo.Id)).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AcceptKudo_UserIsNotOwnerOfBoard_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, "boardCreator", "sender", "receiver", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Accept(userId, kudo.Id)).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AcceptKudo_AcceptedKudo_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.Accepted;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Accept(userId, kudo.Id)).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AcceptKudo_RejectedKudo_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.Rejected;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Accept(userId, kudo.Id)).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void RejectKudo_RejectedKudo_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.Rejected;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Reject(userId, kudo.Id)).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void RejectKudo_AcceptedKudo_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.Accepted;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Reject(userId, kudo.Id)).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void AcceptKudo_KudoAccepted_SenderGetsNotificationAboutAcceptation()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, userId, "sender", "receiver", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            Manager.Accept(userId, kudo.Id);

            var notification = DbContext.Notifications.SingleOrDefault(x => x.ReceiverId == "sender" && x.Type == NotificationTypes.KudoAccepted);
            notification.Should().NotBeNull();
            notification.SenderId.Should().Be("user");
        }

        [Fact]
        public void AcceptKudo_KudoAccepted_ReceiverGetsNotificationAboutKudoAdded()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, userId, "sender", "receiver", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            Manager.Accept(userId, kudo.Id);

            var notification = DbContext.Notifications.SingleOrDefault(x => x.ReceiverId == "receiver" && x.Type == NotificationTypes.KudoAdded);
            notification.Should().NotBeNull();
            notification.SenderId.Should().Be("sender");
        }

        [Fact]
        public void RejectKudo_KudoRejected_SenderGetsNotificationAboutKudoRejection()
        {
            string userId = "user";
            var kudo = KudosHelper.CreateKudo(1, userId, "sender", "receiver", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            Manager.Reject(userId, kudo.Id);

            var notification = DbContext.Notifications.SingleOrDefault(x => x.ReceiverId == "sender" && x.Type == NotificationTypes.KudoRejected);
            notification.Should().NotBeNull();
            notification.SenderId.Should().Be("user");
        }
    }
}
