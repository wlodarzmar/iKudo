using iKudo.Domain.Interfaces;
using iKudo.Domain.Logic;
using iKudo.Domain.Model;

namespace iKudo.Domain.Tests.Joins
{
    public class JoinRequestTestsBase : BaseTest
    {
        public JoinRequestTestsBase()
        {
            Manager = new JoinManager(DbContext, TimeProviderMock.Object);
        }

        public IManageJoins Manager { get; set; }

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
