using iKudo.Domain.Criteria;
using iKudo.Domain.Enums;
using Moq;
using Xunit;

namespace iKudo.Clients.Web.Tests
{
    public class JoinRequestControllerGetTests : JoinRequestControllerTestsBase
    {
        [Fact]
        public void GetJoinRequests_WithGivenCandidateId_CallsGetJoinsWithCandidateId()
        {
            string userId = "userId";

            Controller.GetJoinRequests(new JoinSearchCriteria { CandidateId = userId });

            JoinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.CandidateId == userId)));
        }

        [Fact]
        public void GetJoinRequest_ForGivenBoardId_CallsGetJoinsWithBoardId()
        {
            int boardId = 1;

            Controller.GetJoinRequests(new JoinSearchCriteria { BoardId = boardId });

            JoinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.BoardId == boardId)));
        }

        [Fact]
        public void GetJoinRequest_WithGivenStatus_CallsGetJoinsWithValidCriteria()
        {
            Controller.GetJoinRequests(new JoinSearchCriteria { BoardId = 1, StatusText = "waiting" });

            JoinManagerMock.Verify(x => x.GetJoins(It.Is<JoinSearchCriteria>(c => c.Status == JoinStatus.Waiting)));
        }
    }
}
