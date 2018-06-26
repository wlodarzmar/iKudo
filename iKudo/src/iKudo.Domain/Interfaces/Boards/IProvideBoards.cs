using iKudo.Domain.Criteria;
using iKudo.Domain.Model;
using System.Collections.Generic;

namespace iKudo.Domain.Interfaces
{
    public interface IProvideBoards
    {
        Board Get(int id);

        ICollection<Board> GetAll(string userId, BoardSearchCriteria criteria);
    }
}
