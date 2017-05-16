using System.Collections.Generic;
using iKudo.Domain.Model;

namespace iKudo.Domain.Interfaces
{
    public interface IBoardManager
    {
        Board Add(Board board);

        Board Get(int id);

        ICollection<Board> GetAll();

        void Delete(string userId, int id);

        void Update(Board board);

        JoinRequest Join(int boardId, string candidateId);

        ICollection<JoinRequest> GetJoinRequests(string v);
    }
}
