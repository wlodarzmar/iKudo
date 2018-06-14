using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace iKudo.Domain.Interfaces
{
    public interface IManageBoards
    {
        Board Add(Board board);

        Board Get(int id);

        ICollection<Board> GetAll(string userId, BoardSearchCriteria criteria);

        void Delete(string userId, int id);

        void Update(Board board);

        Task Invite(string user, int boardId, string[] emails);
    }
}
