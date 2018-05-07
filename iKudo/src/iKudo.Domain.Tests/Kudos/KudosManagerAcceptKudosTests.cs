using FluentAssertions;
using iKudo.Domain.Enums;
using System;
using Xunit;

namespace iKudo.Domain.Tests.Kudos
{
    public class KudosManagerAcceptKudosTests : KudosManagerBaseTest
    {
        [Fact]
        public void AcceptKudo_NewKudo_KudoAccepted()
        {
            string userId = "user";
            var kudo = CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            var acceptedKudo = Manager.Accept(userId, kudo.Id);

            acceptedKudo.Status.Should().Be(KudoStatus.Accepted);
        }

        [Fact]
        public void RejectKudo_NewKudo_KudoRejected()
        {
            string userId = "user";
            var kudo = CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            var acceptedKudo = Manager.Reject(userId, kudo.Id);

            acceptedKudo.Status.Should().Be(KudoStatus.Rejected);
        }

        [Fact]
        public void RejectKudo_UserIsNotOwnerOfBoard_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = CreateKudo(1, "boardCreator", "sender", "receiver", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Reject(userId, kudo.Id)).ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AcceptKudo_UserIsNotOwnerOfBoard_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = CreateKudo(1, "boardCreator", "sender", "receiver", false);
            kudo.Status = KudoStatus.New;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Accept(userId, kudo.Id)).ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AcceptKudo_AcceptedKudo_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.Accepted;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Accept(userId, kudo.Id)).ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void AcceptKudo_RejectedKudo_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.Rejected;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Accept(userId, kudo.Id)).ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void RejectKudo_RejectedKudo_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.Rejected;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Reject(userId, kudo.Id)).ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void RejectKudo_AcceptedKudo_InvalidOperationExceptionIsThrown()
        {
            string userId = "user";
            var kudo = CreateKudo(1, userId, "sender", false);
            kudo.Status = KudoStatus.Accepted;
            DbContext.Fill(kudo);

            Manager.Invoking(x => x.Reject(userId, kudo.Id)).ShouldThrow<InvalidOperationException>();
        }
    }
}
