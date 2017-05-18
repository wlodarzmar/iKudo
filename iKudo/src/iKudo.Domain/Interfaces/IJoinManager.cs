using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IJoinManager
    {
        JoinRequest Join(int boardId, string candidateId);

        ICollection<JoinRequest> GetJoinRequests(string userId);
    }
}
