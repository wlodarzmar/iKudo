using FluentAssertions;
using iKudo.Domain.Criteria;
using iKudo.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
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
            KudosSearchCriteria criteria = parser.Parse(null, null, null, null, null);

            criteria.Should().NotBeNull();
        }

        [Theory]
        [InlineData("currUser", 1)]
        [InlineData("currUser", null)]
        [InlineData(null, 2)]
        public void Parse_WithCurrentUserAndBoardId_ReturnsCriteriaWithProperlySettedProperties(string currentUser, int? boardId)
        {
            KudosSearchCriteria criteria = parser.Parse(currentUser, boardId, null, null, null);

            criteria.BoardId.Should().Be(boardId);
            criteria.UserPerformingActionId.Should().Be(currentUser);
        }

        [Fact]
        public void Parse_WithSender_ReturnsUserSearchTypeAsSenderOnly()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, "sender", null, null);

            criteria.User.Should().Be("sender");
            criteria.UserSearchType.Should().Be(UserSearchTypes.SenderOnly);
        }

        [Fact]
        public void Parse_WithReceiver_ReturnsUserSearchTypeAsReceiverOnly()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, null, "receiver", null);

            criteria.User.Should().Be("receiver");
            criteria.UserSearchType.Should().Be(UserSearchTypes.ReceiverOnly);
        }

        [Fact]
        public void Parse_WithUser_ReturnsUserSearchTypeAsBoth()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, null, null, "user");

            criteria.User.Should().Be("user");
            criteria.UserSearchType.Should().Be(UserSearchTypes.Both);
        }

        [Fact]
        public void Parse_WithSameSenderAndReceiver_SetsUserAndUserSearchTypeToBoth()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, "someUser", "someUser", null);

            criteria.User.Should().Be("someUser");
            criteria.UserSearchType.Should().Be(UserSearchTypes.Both);
        }

        [Fact]
        public void Parse_WithDifferentSenderAndReceiver_ThrowsArgumentException()
        {
            parser.Invoking(x => x.Parse(null, null, "sender", "receiver", null))
                  .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void Parse_IfSenderOrReceiverIsPassed_ItIgnoresSenderAndReceiverArguments()
        {
            KudosSearchCriteria criteria = parser.Parse(null, null, "sender", "receiver", "user");

            criteria.User.Should().Be("user");
            criteria.UserSearchType.Should().Be(UserSearchTypes.Both);
        }
    }
}
