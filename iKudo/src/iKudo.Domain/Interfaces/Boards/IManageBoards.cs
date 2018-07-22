using iKudo.Domain.Model;
using System.Threading.Tasks;

namespace iKudo.Domain.Interfaces
{
    public interface IManageBoards
    {
        Board Add(Board board);

        void Delete(string userId, int id);

        void Update(Board board);

        Task Invite(string user, int boardId, string[] emails);

        void AcceptInvitation(string userId, int boardId, string code);
    }
}
