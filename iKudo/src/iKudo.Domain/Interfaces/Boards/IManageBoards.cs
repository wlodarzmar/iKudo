using iKudo.Domain.Model;

namespace iKudo.Domain.Interfaces
{
    public interface IManageBoards
    {
        Board Add(Board board);

        void Delete(string userId, int id);

        void Update(Board board);
    }
}
