using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using iKudo.Parsers;
using System;
using System.Linq;
using Xunit;

namespace iKudo.Clients.Web.Tests.ParsersTests
{
    public class KudoSearchCriteriaParserTests
    {
        private readonly IKudoSearchCriteriaParser parser;

        public KudoSearchCriteriaParserTests()
        {
            parser = new KudoSearchCriteriaParser();
        }

        [Fact]
        public void Parse_NullParameters_ReturnsDefaultCriteria()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, null, null, null, null);

            criteria.Should().NotBeNull();
        }

        [Theory]
        [InlineData("currUser", 1)]
        [InlineData("currUser", null)]
        [InlineData(null, 2)]
        public void Parse_WithCurrentUserAndBoardId_ReturnsCriteriaWithProperlySettedProperties(string currentUser, int? boardId)
        {
            KudosSearchCriteria criteria = parser.Parse(currentUser, boardId, null, null, null, null);

            criteria.BoardId.Should().Be(boardId);
            criteria.UserPerformingActionId.Should().Be(currentUser);
        }

        [Fact]
        public void Parse_WithSender_ReturnsUserSearchTypeAsSenderOnly()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, "sender", null, null, null);

            criteria.User.Should().Be("sender");
            criteria.UserSearchType.Should().Be(UserSearchTypes.SenderOnly);
        }

        [Fact]
        public void Parse_WithReceiver_ReturnsUserSearchTypeAsReceiverOnly()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, null, "receiver", null, null);

            criteria.User.Should().Be("receiver");
            criteria.UserSearchType.Should().Be(UserSearchTypes.ReceiverOnly);
        }

        [Fact]
        public void Parse_WithUser_ReturnsUserSearchTypeAsBoth()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, null, null, "user", null);

            criteria.User.Should().Be("user");
            criteria.UserSearchType.Should().Be(UserSearchTypes.Both);
        }

        [Fact]
        public void Parse_WithSameSenderAndReceiver_SetsUserAndUserSearchTypeToBoth()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, "someUser", "someUser", null, null);

            criteria.User.Should().Be("someUser");
            criteria.UserSearchType.Should().Be(UserSearchTypes.Both);
        }

        [Fact]
        public void Parse_WithDifferentSenderAndReceiver_ThrowsArgumentException()
        {
            parser.Invoking(x => x.Parse(null, null, "sender", "receiver", null, null))
                  .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Parse_IfSenderOrReceiverIsPassed_ItIgnoresSenderAndReceiverArguments()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, "sender", "receiver", "user", null);

            criteria.User.Should().Be("user");
            criteria.UserSearchType.Should().Be(UserSearchTypes.Both);
        }

        [Fact]
        public void Parse_KudoStatus_ReturnsProperStatusEnum()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, "sender", "receiver", "user", "accepted");

            criteria.Statuses.Contains(KudoStatus.Accepted).Should().BeTrue();
        }

        [Fact]
        public void Parse_MultipleKudoStatuses_ReturnsCollectionOfStatuses()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, "sender", "receiver", "user", "accepted, new");

            criteria.Statuses.Count().Should().Be(2);
            criteria.Statuses.Contains(KudoStatus.Accepted).Should().BeTrue();
            criteria.Statuses.Contains(KudoStatus.New).Should().BeTrue();
        }
    }
}
