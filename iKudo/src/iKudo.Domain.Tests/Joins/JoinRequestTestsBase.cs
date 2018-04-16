using iKudo.Domain.Model;

namespace iKudo.Domain.Tests.Joins
{
    public class JoinRequestTestsBase : BaseTest
    {
        protected JoinRequest CreateJoinRequest(int boardId, string candidateId)
        {
            return new JoinRequest
            {
                Board = new Board { Id = boardId },
                BoardId = boardId,
                Candidate = new User { Id = candidateId },
                CandidateId = candidateId
            };
        }
    }
}
